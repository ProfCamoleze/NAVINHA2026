using UnityEngine;
public class AtaqueNave : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject laser;
    public Transform pontoDisparo;

    [Header("Recarga")]
    public int disparosMaximos = 5;
    public float tempoRecarga = 1.5f;



    private int disparosAtuais;
    private float timerRecarga;

    private AudioSource somLaser;
   NavinhaControls controles;
    private void Awake()
    {
        controles = new NavinhaControls();
    }
    void Start()
    {
        somLaser = GetComponent<AudioSource>();
        // A nave começa com todos os disparos disponíveis.
        disparosAtuais = disparosMaximos;
        
    }

    private void OnEnable()
    {
        
        controles.Nave.Enable();
    }

void OnDisable()
{
    controles.Nave.Disable();
}

void Update()
    {
        // --------------------------------------
        // RECARGA DOS DISPAROS
        // --------------------------------------

        if (disparosAtuais < disparosMaximos)
        {
            timerRecarga = timerRecarga + Time.deltaTime;

            if (timerRecarga >= tempoRecarga)
            {
                disparosAtuais = disparosAtuais + 1;

                timerRecarga = 0f;
            }
        }

        // --------------------------------------
        // REALIZAR O DISPARO
        // --------------------------------------

        bool apertouAtirar = controles.Nave.Atirar.WasPressedThisFrame();

        if (apertouAtirar && disparosAtuais > 0)
        {
            Instantiate(laser, pontoDisparo.position, pontoDisparo.rotation);

            somLaser.Play();

            disparosAtuais = disparosAtuais - 1;
        }
    }

}