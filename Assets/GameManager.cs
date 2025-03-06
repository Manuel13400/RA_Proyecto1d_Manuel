using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

public class GameManager : MonoBehaviour
{
    TextMeshProUGUI listaEventosCanvas;

    string[] eventos = new string[4];
    int inicioCola, finCola;
    bool planosActivados = true;
    bool eventosActivados = true;

    GameObject eventsList;
    TextMeshProUGUI positionText;

    public GameObject[] prefabList;
    int positionInList;

    TextMeshProUGUI fingersText;
    int fingersInScreen;

    TextMeshProUGUI fingerTextState;

    public ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public ARPlaneManager planeManager;

    void Start()
    {
        // Crea la lista de eventos y establece el inicio y el fin
        for (int i = 0; i < eventos.Length; i++)
        {
            eventos[i] = "";
        }
        inicioCola = 0;
        finCola = 3;

        // Busca los elementos de la interfaz
        listaEventosCanvas = GameObject.Find("EventsListText").GetComponent<TextMeshProUGUI>();
        eventsList = GameObject.Find("EventsList");

        positionInList = 0;
        positionText = GameObject.Find("PositionText").GetComponent<TextMeshProUGUI>();

        fingersText = GameObject.Find("FingersText").GetComponent<TextMeshProUGUI>();

        fingerTextState = GameObject.Find("FingerState").GetComponent<TextMeshProUGUI>();

    }

    // Instancia el prefab escogido en el plano
    public void EventoPrefabInstanciado()
    {
        AddEvent("Prefab " + (positionInList + 1) + " Inst.");
        Vector2 centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);

        if (raycastManager.Raycast(centerScreen, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose pose = hits[0].pose;
            Instantiate(prefabList[positionInList], pose.position, pose.rotation);
        }
    }

    // Al pulsar se dejan de generar planos o se vuelven a generar
    public void EventoPlanosActivados()
    {
        if (planosActivados)
        {
            AddEvent("Planos Act."); planosActivados = false;
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
        else
        {
            AddEvent("Planos Desact."); planosActivados = true;
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
    
    // Activa y desactiva la lista de eventos (solo visualmente, los eventos se siguen guardando)
    public void EventoEventosActivados()
    {
        if (eventosActivados)
        {
            AddEvent("Eventos Desact."); eventosActivados = false;
            eventsList.SetActive(false);
        }
        else
        {
            AddEvent("Eventos Act."); eventosActivados = true;
            eventsList.SetActive(true);
        }
    }

    // Boton para elegir el siguiente prefab (si supera el limite vuelve a la posicion inicial)
    public void EventoSiguientePrefab()
    {
        AddEvent("Sig. Prefab");
        positionInList++;
        if (positionInList > 2)
        {
            positionInList = 0;
        }
    }

    // Boton para elegir el prefab anterior (si supera el limite vuelve a la posicion final)
    public void EventoAnteriorPrefab()
    {
        AddEvent("Ant. Prefab");
        positionInList--;
        if (positionInList < 0)
        {
            positionInList = 2;
        }
    }

    // Añade evento a la lista de eventos
    void AddEvent(string newEvent)
    {
        inicioCola++;
        finCola++;

        if (inicioCola >= eventos.Length) { inicioCola = 0; }
        if (finCola >= eventos.Length) { finCola = 0; }

        eventos[finCola] = newEvent;
    }

    // Muestra la lista de eventos
    void ShowEventsList()
    {
        string textoCola = "";

        if (inicioCola == 0)
        {
            for (int i = 0; i < eventos.Length; i++)
            {
                textoCola = textoCola + (i + 1) + ". " + eventos[i] + "\n";
            }
        }
        else
        {
            for (int i = inicioCola; i < eventos.Length; i++)
            {
                textoCola = textoCola + (i + 1) + ". " + eventos[i] + "\n";
            }

            for (int i = 0; i <= finCola; i++)
            {
                textoCola = textoCola + (i + 1) + ". " + eventos[i] + "\n";
            }
        }

        listaEventosCanvas.text = textoCola;
    }

    // Indica en pantalla los estados del primer dedo que toque la pantalla
    void checkFingerStatus()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    fingerTextState.text = "Dedo en pantalla";
                    break;
                case TouchPhase.Moved:
                    fingerTextState.text = "Dedo moviéndose";
                    break;
                case TouchPhase.Stationary:
                    fingerTextState.text = "Dedo quieto";
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    fingerTextState.text = "Ya no hay dedo";
                    break;
            }
        }
        else
        {
            fingerTextState.text = "No hay dedos";
        }
    }

    void Update()
    {
        fingersInScreen = Input.touchCount;
        fingersText.text = "Dedos: " + fingersInScreen.ToString();

        ShowEventsList();
        positionText.text = (positionInList + 1).ToString();

        checkFingerStatus();
    }
}
