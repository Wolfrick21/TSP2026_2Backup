using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class Firestoreinicialize : MonoBehaviour
{
    private static FirebaseFirestore firestore;
    [SerializeField]
    private TMP_InputField cardNameInput;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        firestore = FirebaseFirestore.DefaultInstance;
    }

    public void FetchAndStoreCardDta()
    {
        string cardName = cardNameInput.text;
        StartCoroutine(GetCardData(cardName));
    }

    private IEnumerator GetCardData(string cardName)
    {
        string url = $"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={UnityWebRequest.EscapeURL(cardName)}";

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            ProcessCardData(json);
        }
        else
        {
            Debug.LogError("Error obteniendo datos: "+request.error);
        }
    }

    private void ProcessCardData(string json)
    {
        //Parsero JSON data
        var cardData = JsonUtility.FromJson<CardDataResponse>(json);

        if(cardData.data.Length>0)
        {
            var card = cardData.data[0];
            string cardType = card.type;
            //Estructura en firestore
            string collection = DetermineCollection(cardType);
            string cardName = card.name;

            //crear documento de referencia basado en el tipo de carta y su nombre
            DocumentReference docRef = firestore
                .Collection("Cartas")
                .Document(collection)
                .Collection(cardName)
                .Document("Datos");

            //reparar datos para guardar en Firestore
            var cardinfo = new Dictionary<string, object>
                {
                    {"ATK",card.atk },
                    {"DEF",card.def},
                    {"Desc",card.desc},
                    {"Level",card.level}
                };
            //Guardar datos en Firestore

            docRef.SetAsync(cardinfo).ContinueWithOnMainThread(task => 
            {
                if(task.IsCompleted)
                {
                    Debug.Log($"Datos de {cardName} alamacenados correctamente");
                }
                else
                {
                    Debug.LogError("Error guardando datos" + task.ToString());
                }
            });
        }
        else
        {
            Debug.LogError("Carta no encontrada");
        }
    }

    private string DetermineCollection(string cardType)
    {
        if(cardType.Contains("Monster"))
        {
            return "Mounstruo";
        }
        if(cardType.Contains("Spell"))
        {
            return "Magia";
        }
        if(cardType.Contains("Trap"))
        {
            return "Trampa";
        }
        else
        {
            return "Otros";
        }
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    //Calse para mapear la respuesta tipo JSON
    [System.Serializable]
    public class CardDataResponse
    {
        public CardData[] data;
    }
    [System.Serializable]
    public class CardData
    {
        public string name;
        public string type;
        public string desc;
        public int atk;
        public int def;
        public int level;
    }
}
