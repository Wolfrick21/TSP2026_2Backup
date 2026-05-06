using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;

public class BD : MonoBehaviour
{
    public DatabaseReference reference;
    [SerializeField]
    TMP_InputField textoNombre;
    [SerializeField]
    TMP_InputField textoEdad;
    public bool registroBooleano = true;

    private void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Booleano(bool toggleB)
    {
        registroBooleano = toggleB;
    }

    public void Registro()
    {
        //Generar clave para registro
        string key = reference.Child("Nombre").Push().Key;
        reference.Child("Nombre").Child(key).SetValueAsync(textoNombre.text);

        //Clave unica para datos individuales
        reference.Child("Edad").SetValueAsync(int.Parse(textoEdad.text));

        //Clave para tipo booleano
        reference.Child("Booleano").SetValueAsync(registroBooleano);

        //Clave para registro de objeto tipo Usuario
        Usuario usuario = new Usuario("Mario", "mario@gmail.com");
        string json = JsonUtility.ToJson(usuario);

        reference.Child("Usuario").SetRawJsonValueAsync(json);

        //Datos a actualizar
        Debug.Log("Dato Escuela anterior: UNAM");
        reference.Child("Escuela").SetValueAsync("UNAM");
        Debug.Log("Dato Escuela actual: POLI");
        reference.Child("Escuela").SetValueAsync("POLI");

        Debug.Log("Dato Año anterior: 2025");
        reference.Child("Año").SetValueAsync(2025);
        Debug.Log("Dato Año actual: 2026");
        reference.Child("Año").SetValueAsync(2026);
    }

    public void CargaBD()
    {
        //Obteniendo registro Año
        reference.Child("Año").GetValueAsync().ContinueWithOnMainThread
        (
            TaskExtension =>
            {
                if(TaskExtension.IsFaulted)
                {
                    Debug.Log("Error al obtener datos" +TaskExtension.Exception);
                }
                else if (TaskExtension.IsCompleted)
                {
                    DataSnapshot snapshot = TaskExtension.Result;
                    string value = snapshot.Value.ToString();
                    Debug.Log("Tipo de valor obtenido: "+snapshot.Value.GetType());
                    Debug.Log("Valor: "+value);
                }
                else
                {
                    Debug.Log("Registro no encontrado");
                }
            }
        );

        //Carga de valore anidados con clave unica
        reference.Child("Nombre").GetValueAsync().ContinueWithOnMainThread
        (
            TaskExtension =>
            {
                if (TaskExtension.IsFaulted)
                {
                    Debug.Log("Error al obtener datos" + TaskExtension.Exception);
                }
                else if (TaskExtension.IsCompleted)
                {
                    DataSnapshot snapshot2 = TaskExtension.Result;
                    //Recorrer todos los hijos de Registro Nombre y obtener los valore
                    foreach(DataSnapshot childSnapshot in snapshot2.Children)
                    {
                        string value2 = childSnapshot.Value.ToString();
                        Debug.Log("Tipo de valor obtenido: " + childSnapshot.Value.GetType());
                        Debug.Log("Valor: " + value2);
                    }
                    
                }
                else
                {
                    Debug.Log("Registro no encontrado");
                }
            }
        );

        //Carga tipo JSON
        reference.Child("Usuario").GetValueAsync().ContinueWithOnMainThread
        (
            TaskExtension =>
            {
                if (TaskExtension.IsFaulted)
                {
                    Debug.Log("Error al obtener datos" + TaskExtension.Exception);
                }
                else if (TaskExtension.IsCompleted)
                {
                    DataSnapshot snapshot3 = TaskExtension.Result;
                    //Convertir el JSON a un diccionario
                    Dictionary<string, object> userData = JsonConvert.DeserializeObject<Dictionary<string, object>>(snapshot3.GetRawJsonValue());
                    string nombre = (string)userData["UserName"];
                    string email = (string)userData["Email"];

                    Debug.Log($"Nombre de usuario {nombre}, correo {email}");
                }
                else
                {
                    Debug.Log("Registro no encontrado");
                }
            }
        );
    }
}

public class Usuario
{
    public string UserName;
    public string Email;

    public Usuario (string username, string email)
    {
        this.UserName = username;
        this.Email = email;
    }
}
