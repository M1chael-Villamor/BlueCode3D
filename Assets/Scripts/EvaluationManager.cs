using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EvaluationManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtEnunciado;
    public TextMeshProUGUI txtDominio;
    public TextMeshProUGUI txtContadorPregunta; // Nuevo: Texto para "Pregunta X de Y" o similar
    public TextMeshProUGUI[] txtOpcionesTexts;  // Los textos de los 4 botones
    public Button[] btnOpciones;                // Los 4 botones de opciones en sí
    public Button btnSiguiente;                 // El botón Siguiente
    public Button btnAtras;                     // Nuevo: El botón Atrás

    [Header("Visual Settings for Selection")]
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = new Color(0.3f, 0.7f, 1f); // Tono azul claro destacado

    private BancoPreguntas banco;
    private int indicePreguntaActual = 0;
    private int opcionSeleccionada = -1;

    void Start()
    {
        if (btnSiguiente != null) btnSiguiente.gameObject.SetActive(false);
        if (btnAtras != null) btnAtras.gameObject.SetActive(false);

        CargarBancoPreguntas();
        MostrarPregunta();
    }

    void CargarBancoPreguntas()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("preguntas");
        if (jsonFile != null)
        {
            banco = JsonUtility.FromJson<BancoPreguntas>(jsonFile.text);
        }
        else
        {
            Debug.LogError("No se encontró el archivo preguntas.json en Assets/Resources/");
        }
    }

    void MostrarPregunta()
    {
        if (banco == null || banco.preguntas == null) return;

        // Si llegamos al final, reiniciamos para poder seguir probando cómodamente
        if (indicePreguntaActual >= banco.preguntas.Count)
        {
            Debug.Log("¡Fin de la prueba! Reiniciando desde la primera pregunta.");
            indicePreguntaActual = 0;
        }

        Pregunta p = banco.preguntas[indicePreguntaActual];
        
        if (txtEnunciado != null) txtEnunciado.text = p.enunciado;
        if (txtDominio != null) txtDominio.text = $"Dominio: {p.dominio}";
        
        // Actualizamos el contador de la pregunta (Ej: Pregunta 15 / 40)
        if (txtContadorPregunta != null) 
            txtContadorPregunta.text = $"Pregunta {indicePreguntaActual + 1} de {banco.preguntas.Count}";

        for (int i = 0; i < txtOpcionesTexts.Length; i++)
        {
            if (i < p.opciones.Count && txtOpcionesTexts[i] != null)
                txtOpcionesTexts[i].text = p.opciones[i];
        }

        RestaurarColoresBotones();
        opcionSeleccionada = -1;
        if (btnSiguiente != null) btnSiguiente.gameObject.SetActive(false);

        // Controlamos la visibilidad del botón Atrás: solo visible a partir de la segunda pregunta (índice 1 en adelante)
        if (btnAtras != null)
        {
            btnAtras.gameObject.SetActive(indicePreguntaActual > 0);
        }
    }

    // Vinculado a los 4 botones de opciones
    public void SeleccionarOpcion(int indiceOpcion)
    {
        opcionSeleccionada = indiceOpcion;
        
        for (int i = 0; i < btnOpciones.Length; i++)
        {
            if (btnOpciones[i] == null) continue;

            Image imgButton = btnOpciones[i].GetComponent<Image>();
            if (imgButton != null)
            {
                if (i == indiceOpcion)
                {
                    imgButton.color = colorSeleccionado;
                }
                else
                {
                    imgButton.color = colorNormal;
                }
            }
        }

        if (btnSiguiente != null) 
            btnSiguiente.gameObject.SetActive(true);
    }

    void RestaurarColoresBotones()
    {
        foreach (var btn in btnOpciones)
        {
            if (btn != null)
            {
                Image imgButton = btn.GetComponent<Image>();
                if (imgButton != null)
                {
                    imgButton.color = colorNormal;
                }
            }
        }
    }

    // Vinculado al botón Siguiente
    public void SiguientePregunta()
    {
        if (opcionSeleccionada == -1) return;

        indicePreguntaActual++;
        MostrarPregunta();
    }

    // Nuevo: Vinculado al botón Atrás
    public void PreguntaAnterior()
    {
        if (indicePreguntaActual > 0)
        {
            indicePreguntaActual--;
            MostrarPregunta();
        }
    }
}