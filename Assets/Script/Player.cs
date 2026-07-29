using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movimento")]
    public float aceleracao = 8f;           // força aplicada ao acelerar
    public float velocidadeMaxima = 6f;    // limite de velocidade da nave
    public float frenagem = 4f;           // quão rápido a velocidade cai ao frear
    public float velocidadeRotacao = 720f; // graus/seg. de giro pra mirar no mouse

    [Header("Referências")]
    public Camera cameraDoJogo;

    [Header("Tiro")]
    public GameObject laser;
    public Transform disparo;
    public AudioSource somLaser;
    public int disparosMaximos = 5;     // "pente" de tiros (3 a 5, ajuste no Inspector)
    public float tempoRecarga = 1.5f;    // segundos para recuperar 1 disparo

    [Header("Camera Shake")]
    public CinemachineImpulseSource fonteDeImpacto; // dispara o tremor de câmera

    int disparosAtuais;      // quantos tiros a nave tem "no pente" agora
    float timerRecarga;      // contador de tempo até recuperar 1 tiro

    Rigidbody2D rig;
    Vector2 direcaoMira;      // direção (normalizada) do Player até o mouse
    bool estaAcelerando;      // true enquanto a Seta ↑ estiver apertada
    bool estaFreando;         // true enquanto a Seta ↓ estiver apertada

    NavinhaControls controles; // classe gerada pelo Input Actions (Módulo 03)

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        controles = new NavinhaControls();
    }

    void OnEnable()
    {
        // "Enable" liga o mapa de controles — sem isso, os inputs não são lidos.
        // Repare que aqui NÃO usamos += nem lambda: a gente só liga o mapa,
        // e vai ficar checando os botões manualmente dentro do Update().
        controles.Nave.Enable();
    }

    void OnDisable()
    {
        // sempre desliga junto — evita erros quando o objeto é destruído
        controles.Nave.Disable();
    }

    void Start()
    {
        somLaser = GetComponent<AudioSource>();
        disparosAtuais = disparosMaximos; // a nave começa com o pente cheio
    }

    void Update()
    {
        LerInput();
        GerenciarTiro(); // cuida do disparo E da recarga, tudo num lugar só
    }

    void FixedUpdate()
    {
        // física sempre no FixedUpdate, pra ficar suave e independente de FPS
        Mirar();
        Mover();
    }

    void LerInput()
    {
        // IsPressed() responde true enquanto a tecla estiver SEGURADA.
        // Guardamos o resultado em variáveis para usar depois, no FixedUpdate.
        estaAcelerando = controles.Nave.Acelerar.IsPressed(); // Seta para Cima
        estaFreando = controles.Nave.Frear.IsPressed();       // Seta para Baixo
    }

    void Mirar()
    {
        Vector2 posicaoMouseTela = controles.Nave.Mirar.ReadValue<Vector2>();
        Vector2 posicaoMouseMundo = cameraDoJogo.ScreenToWorldPoint(posicaoMouseTela);

        direcaoMira = (posicaoMouseMundo - rig.position).normalized;
        float anguloAlvo = Mathf.Atan2(direcaoMira.y, direcaoMira.x) * Mathf.Rad2Deg;

        // MoveTowardsAngle gira suavemente até o ângulo alvo, em vez de "teletransportar" a rotação
        float anguloSuave = Mathf.MoveTowardsAngle(rig.rotation, anguloAlvo, velocidadeRotacao * Time.fixedDeltaTime);
        rig.MoveRotation(anguloSuave);
    }

    void Mover()
    {
        if (estaAcelerando == true)
        {
            // Seta ↑ apertada = empurra a nave na direção da mira (o ponteiro do mouse)
            Vector2 empurrao = direcaoMira * aceleracao * Time.fixedDeltaTime;
            rig.linearVelocity = rig.linearVelocity + empurrao;

            // ClampMagnitude impede que a nave passe da velocidade máxima
            rig.linearVelocity = Vector2.ClampMagnitude(rig.linearVelocity, velocidadeMaxima);
        }
        else if (estaFreando == true)
        {
            // Seta ↓ apertada = reduz a velocidade aos poucos, até chegar em zero.
            // Não é uma parada brusca: quanto maior "frenagem", mais rápido a nave para.
            rig.linearVelocity = Vector2.MoveTowards(rig.linearVelocity, Vector2.zero, frenagem * Time.fixedDeltaTime);
        }

        // Se nenhuma seta estiver apertada, a nave simplesmente mantém
        // a velocidade atual — é a "inércia" do espaço.
    }

    void GerenciarTiro()
    {
        // ---- PARTE 1: recarga -----------------------------------------
        // se o pente ainda não está cheio, vai contando o tempo até liberar mais 1 tiro
        if (disparosAtuais < disparosMaximos)
        {
            // soma o tempo do frame ao contador
            timerRecarga = timerRecarga + Time.deltaTime;

            if (timerRecarga >= tempoRecarga)
            {
                disparosAtuais = disparosAtuais + 1; // ganhou 1 tiro de volta
                timerRecarga = 0f;                   // zera o cronômetro
            }
        }

        // ---- PARTE 2: disparo -------------------------------------------
        // WasPressedThisFrame() é true só no exato frame em que o botão foi apertado
        bool apertouAtirar = controles.Nave.Atirar.WasPressedThisFrame();

        if (apertouAtirar == true && disparosAtuais > 0)
        {
            Instantiate(laser, disparo.position, disparo.rotation);
            somLaser.Play();
            disparosAtuais = disparosAtuais - 1; // gastou 1 tiro
        }
    }

    void OnTriggerEnter2D(Collider2D bateu)
    {
        // mantém o "teleporte" nas bordas da tela, igual o original
        if (bateu.gameObject.CompareTag("x"))
            transform.position = new Vector3(transform.position.x * -0.9f, transform.position.y, transform.position.z);

        if (bateu.gameObject.CompareTag("y"))
            transform.position = new Vector3(transform.position.x, transform.position.y * -0.9f, transform.position.z);
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (colisao.gameObject.CompareTag("Asteroid"))
        {
            // Só chama GenerateImpulse() se o campo estiver preenchido no Inspector.
            // Se estiver vazio (null), o jogo daria erro — então checamos antes.
            if (fonteDeImpacto != null)
            {
                fonteDeImpacto.GenerateImpulse(); // faz a câmera tremer!
            }
        }
    }
}