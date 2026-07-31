using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MoverNave : MonoBehaviour
{
    [Header("Movimento")]
    public float aceleracao = 8f;
    public float velocidadeMaxima = 6f;
    public float frenagem = 4f;
    public float velocidadeRotacao = 720f;

    [Header("Referências")]
    public Camera cameraDoJogo;

    private Rigidbody2D rig;

    // O script de ataque também poderá acessar os controles.
NavinhaControls Controles;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        Controles = new NavinhaControls();

        // Se nenhuma câmera for indicada, procura a câmera principal.

        cameraDoJogo = Camera.main;

    }

    void OnEnable()
    {
        Controles.Nave.Enable();
    }

    void OnDisable()
    {
        Controles.Nave.Disable();
    }

    void FixedUpdate()
    {
        // --------------------------------------
        // MIRAR PARA O MOUSE
        // --------------------------------------

        Vector2 posicaoMouseTela = Controles.Nave.Mirar.ReadValue<Vector2>();

        Vector2 posicaoMouseMundo = cameraDoJogo.ScreenToWorldPoint(posicaoMouseTela);

        Vector2 direcaoMira = (posicaoMouseMundo - rig.position).normalized;

        float anguloAlvo = Mathf.Atan2(direcaoMira.y, direcaoMira.x) * Mathf.Rad2Deg;

        float anguloSuave = Mathf.MoveTowardsAngle(rig.rotation, anguloAlvo, velocidadeRotacao * Time.fixedDeltaTime);

        rig.MoveRotation(anguloSuave);

        // --------------------------------------
        // ACELERAR E FREAR
        // --------------------------------------

        bool estaAcelerando = Controles.Nave.Acelerar.IsPressed();

        bool estaFreando = Controles.Nave.Frear.IsPressed();

        if (estaAcelerando)
        {
            Vector2 empurrao = direcaoMira * aceleracao * Time.fixedDeltaTime;

            rig.linearVelocity = rig.linearVelocity + empurrao;

            rig.linearVelocity = Vector2.ClampMagnitude(rig.linearVelocity, velocidadeMaxima);
        }
        else if (estaFreando)
        {
            rig.linearVelocity = Vector2.MoveTowards(rig.linearVelocity, Vector2.zero, frenagem * Time.fixedDeltaTime);
        }

        // Sem acelerar ou frear, a nave mantém sua inércia.
    }

    void OnTriggerEnter2D(Collider2D bateu)
    {
        // Teleporte pelas bordas horizontais.
        if (bateu.CompareTag("x"))
        {
            transform.position = new Vector3(transform.position.x * -0.9f, transform.position.y, transform.position.z);
        }

        // Teleporte pelas bordas verticais.
        if (bateu.CompareTag("y"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y * -0.9f, transform.position.z);
        }
    }
}