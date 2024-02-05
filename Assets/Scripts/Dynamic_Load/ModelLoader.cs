using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;


public class ModelLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] root;
    GameObject wrapper;
    private string apiTemplate = "http://localhost:4000/model/{0}/{1}";

    void Start()
    {
        // TODO: Llamada a sistema de recomendacion para obtener modelos y realizar llamadas a la API
        

    }

    public void TriggerLoading()
    {
        int id_producto = 17;
        int id_tienda = 10;
        string apiEndpoint = string.Format(apiTemplate, id_producto, id_tienda);
        StartCoroutine(GetModelUrl(apiEndpoint, root[0]));
        
        id_producto = 11;
        id_tienda = 10;
        apiEndpoint = string.Format(apiTemplate, id_producto, id_tienda);
        StartCoroutine(GetModelUrl(apiEndpoint, root[1]));
    }
    
    [System.Serializable]
    public class ModelResponse
    {
        // TODO: el recurso de la API debe devolver la url como una string y no como una lista
        public string[] modelurl;
    }


    void ResetWrapper()
    {
        if (wrapper != null)
        {
            foreach (Transform trans in wrapper.transform)
            {
                Destroy(trans.gameObject);
            }
        }
    }

    float GetPuntoMasBajo(GameObject obj)
    {
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();

        if (meshRenderer)
        {
            Bounds bounds = meshRenderer.bounds;
            return bounds.min.y; // Devuelve el punto más bajo en el eje Y.
        }

        // Si no tiene un MeshRenderer, retorna la posición y del objeto como punto de referencia.
        return obj.transform.position.y;
        
    }

    void RescaleModel(GameObject model)
    {
        // Asegúrate de que el modelo no esté activo al calcular sus dimensiones
        model.SetActive(false);

        // Obtiene el Renderer del objeto cargado
        Renderer rend = model.GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            // Aquí, determina las dimensiones y decide tu factor de escala basado en ellas.
            float height = rend.bounds.size.y;

            // Por ejemplo, si quieres que todos los modelos tengan una altura de 2 unidades en Unity
            float targetHeight = 0.2f;
            float scaleFactor = targetHeight / height;

            // Aplica el factor de escala
            model.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            // Alinear con el eje Z del padre
            //model.transform.forward = root.transform.forward;

            // Activa el modelo para renderizarlo
            model.SetActive(true);
        }
    }

    IEnumerator GetModelUrl(string endpoint, GameObject root)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(endpoint))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al obtener URL: " + request.error);
            }
            else
            {
                ModelResponse response = JsonUtility.FromJson<ModelResponse>(request.downloadHandler.text);
                Debug.Log(response.modelurl[0]);
                StartCoroutine(DownloadModel(response.modelurl[0], root));
            }
        }
    }
    
    IEnumerator DownloadModel(string modelUrl, GameObject root)
    {
        // Extraer el nombre del archivo de la URL
        Uri uri = new Uri(modelUrl);
        string fileName = System.IO.Path.GetFileName(uri.LocalPath);

        if (!CheckIfModelExists(fileName))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(modelUrl))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error al descargar el modelo: " + request.error);
                }
                else
                {
                    // Guardar el archivo con el nombre extraído
                    string savePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
                    System.IO.File.WriteAllBytes(savePath, request.downloadHandler.data);
                    Debug.Log("Modelo guardado en: " + savePath);

                    LoadModel(savePath, root);
                }
            }
        }
        else
        {
            LoadModel(System.IO.Path.Combine(Application.persistentDataPath, fileName), root);
        }
    }

    void LoadModel(string modelPath, GameObject root)
    {
        // TODO: diferenciar entre .gltf o .glb y .obj para cargarlo con el plugin correspondiente
        ResetWrapper();
        GameObject model = Importer.LoadFromFile(modelPath);
        model.transform.SetParent(root.transform);

        //Take the loaded model to the placeholder position
        model.transform.localPosition = new Vector3(0, 0, 0);

        // Calcula el punto más bajo del objeto cargado.
        float puntoMasBajo = GetPuntoMasBajo(model);

        // Calcula la diferencia en Y.
        float diferenciaY = root.transform.position.y - puntoMasBajo;

        // Ajusta la posición del objeto cargado.
        model.transform.localPosition = new Vector3(0, diferenciaY, 0);
        RescaleModel(model);
    }
    
    private bool CheckIfModelExists(string fileName)
    {
        string localPath = GetLocalFilePath(fileName);
        return System.IO.File.Exists(localPath);
    }
    
    private string GetLocalFilePath(string fileName)
    {
        return System.IO.Path.Combine(Application.persistentDataPath, fileName);
    }
}