using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Dummiesman;
using System.IO.Compression;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class ModelLoaderOBJ : MonoBehaviour
{
    // Start is called before the first frame update
    private Stopwatch stopwatch;
    public GameObject root;
    public GameObject[] roots;
    GameObject wrapper;
    private string apiTemplate = "http://localhost:4000/model/{0}/{1}";
    int[] id_productos_lp = { 201, 206, 211, 216 };
    int[] id_productos_hp = { 200, 205, 210, 215 };
    int[] id_productos_mp = { 202, 207, 212, 217 };
    int[] id_productos_polycam = { 203, 208, 213, 218 };
    int[] id_productos_polycamiPad = { 204, 209, 214, 219 };
    int id_tienda = 40;
    
    private int coroutinesCount;
    private int finishedCoroutinesCount;
    void Start()
    {
        coroutinesCount = 0;
        finishedCoroutinesCount = 0;

        stopwatch = new Stopwatch();
        //string apiEndpoint = string.Format(apiTemplate, id_producto, id_tienda);
        //StartCoroutine(GetModelUrl(apiEndpoint));
        LoadFourHighPoly();
    }

    public void LoadOneLowPoly()
    {
        string apiEndpoint = string.Format(apiTemplate, 201, id_tienda);
        StartCoroutine(GetModelUrl(apiEndpoint));
    }
    public void LoadOneMediumPoly()
    {
        string apiEndpoint = string.Format(apiTemplate, 202, id_tienda);
        StartCoroutine(GetModelUrl(apiEndpoint));
    }
    public void LoadOneHighPoly()
    {
        string apiEndpoint = string.Format(apiTemplate, 200, id_tienda);
        StartCoroutine(GetModelUrl(apiEndpoint));
    }
    public void LoadOnePolycam()
    {
        string apiEndpoint = string.Format(apiTemplate, 203, id_tienda);
        StartCoroutine(GetModelUrl(apiEndpoint));
    }
    public void LoadOnePolycamiPad()
    {
        string apiEndpoint = string.Format(apiTemplate, 204, id_tienda);
        StartCoroutine(GetModelUrl(apiEndpoint));
    }
    public void LoadFourLowPoly()
    {
        int parent = 0;
        foreach (int i in id_productos_lp)
        {
            coroutinesCount++;
            string apiEndpoint = string.Format(apiTemplate, i, id_tienda);
            StartCoroutine(GetModelUrl(apiEndpoint, parent));
            parent++;
        }
    }
    public void LoadFourMediumPoly()
    {
        int parent = 0;
        foreach (int i in id_productos_mp)
        {
            coroutinesCount++;
            string apiEndpoint = string.Format(apiTemplate, i, id_tienda);
            StartCoroutine(GetModelUrl(apiEndpoint, parent));
            parent++;
        }
    }
    public void LoadFourHighPoly()
    {
        int parent = 0;
        foreach (int i in id_productos_hp)
        {
            coroutinesCount++;
            string apiEndpoint = string.Format(apiTemplate, i, id_tienda);
            StartCoroutine(GetModelUrl(apiEndpoint, parent));
            parent++;
        }
    }
    public void LoadFourPolycam()
    {
        int parent = 0;
        foreach (int i in id_productos_polycam)
        {
            coroutinesCount++;
            string apiEndpoint = string.Format(apiTemplate, i, id_tienda);
            StartCoroutine(GetModelUrl(apiEndpoint, parent));
            parent++;
        }
    }
    public void LoadFourPolycamiPad()
    {
        int parent = 0;
        foreach (int i in id_productos_polycamiPad)
        {
            coroutinesCount++;
            string apiEndpoint = string.Format(apiTemplate, i, id_tienda);
            StartCoroutine(GetModelUrl(apiEndpoint, parent));
            parent++;
        }
    }
    
    [System.Serializable]
    public class ModelResponseOBJ
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
            float targetHeight = 0.3f;
            float scaleFactor = targetHeight / height;

            // Aplica el factor de escala
            model.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            // Alinear con el eje Z del padre
            //model.transform.forward = root.transform.forward;

            // Activa el modelo para renderizarlo
            model.SetActive(true);
        }
    }

    IEnumerator GetModelUrl(string endpoint)
    {
        stopwatch.Start();
        using (UnityWebRequest request = UnityWebRequest.Get(endpoint))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al obtener URL: " + request.error);
            }
            else
            {
                ModelResponseOBJ response = JsonUtility.FromJson<ModelResponseOBJ>(request.downloadHandler.text);
                Debug.Log(response.modelurl[0]);
                StartCoroutine(DownloadModel(response.modelurl[0]));
            }
        }
    }
    
    IEnumerator DownloadModel(string modelUrl)
    {
        // Extraer el nombre del archivo de la URL
        Uri uri = new Uri(modelUrl);
        string fileName = Path.GetFileName(uri.LocalPath);
        int indicePunto = fileName.LastIndexOf('.');
        string modelName = fileName.Substring(0, indicePunto);

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
                    string savePath = Path.Combine(Application.persistentDataPath, fileName);
                    File.WriteAllBytes(savePath, request.downloadHandler.data);
                    // Descomprimir
                    string extractPath = Path.Combine(Application.persistentDataPath, modelName);
                    Debug.Log("Save path " + savePath);
                    Debug.Log("Extract path " + extractPath);
                    Descomprimir(savePath, extractPath);

                    LoadModel(extractPath);
                }
            }
        }
        else
        {
            LoadModel(Path.Combine(Application.persistentDataPath, modelName));
        }
        /*stopwatch.Stop();
        double segundos = stopwatch.Elapsed.TotalSeconds;
        Debug.Log("Tiempo transcurrido: " + segundos + " segundos");*/
        // Asegúrate de incrementar finishedCoroutinesCount cuando cada Coroutine termine
        finishedCoroutinesCount++;

        // Si todas las Coroutines han terminado, detén el Stopwatch
        if (finishedCoroutinesCount >= coroutinesCount)
        {
            stopwatch.Stop();
            double segundos = stopwatch.Elapsed.TotalSeconds;
            Debug.Log("Tiempo transcurrido: " + segundos + " segundos");
        }
    }

    void LoadModel(string modelPath)
    {
        // Obtén todos los archivos .obj en el directorio
        string[] objFiles = System.IO.Directory.GetFiles(modelPath, "*.obj");
        
        string[] mtlFiles = System.IO.Directory.GetFiles(modelPath, "*.mtl");
        Debug.Log(mtlFiles[0]);

        if(objFiles.Length == 0)
        {
            Debug.LogError("No se encontró ningún archivo .obj en " + modelPath);
            return;
        }
    
        if(objFiles.Length > 1)
        {
            Debug.LogWarning("Se encontraron múltiples archivos .obj en " + modelPath + ". Cargando el primero.");
        }
        ResetWrapper();
        GameObject model = new OBJLoader().Load(objFiles[0], mtlFiles[0]);
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
    
    IEnumerator GetModelUrl(string endpoint, int parent)
    {
        stopwatch.Start();
        using (UnityWebRequest request = UnityWebRequest.Get(endpoint))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al obtener URL: " + request.error);
            }
            else
            {
                ModelResponseOBJ response = JsonUtility.FromJson<ModelResponseOBJ>(request.downloadHandler.text);
                Debug.Log(response.modelurl[0]);
                StartCoroutine(DownloadModel(response.modelurl[0], parent));
            }
        }
    }
    
    IEnumerator DownloadModel(string modelUrl, int parent)
    {
        // Extraer el nombre del archivo de la URL
        Uri uri = new Uri(modelUrl);
        string fileName = Path.GetFileName(uri.LocalPath);
        int indicePunto = fileName.LastIndexOf('.');
        string modelName = fileName.Substring(0, indicePunto);

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
                    string savePath = Path.Combine(Application.persistentDataPath, fileName);
                    File.WriteAllBytes(savePath, request.downloadHandler.data);
                    // Descomprimir
                    string extractPath = Path.Combine(Application.persistentDataPath, modelName);
                    Debug.Log("Save path " + savePath);
                    Debug.Log("Extract path " + extractPath);
                    Descomprimir(savePath, extractPath);

                    LoadModel(extractPath, parent);
                }
            }
        }
        else
        {
            LoadModel(Path.Combine(Application.persistentDataPath, modelName), parent);
        }
        /*stopwatch.Stop();
        double segundos = stopwatch.Elapsed.TotalSeconds;
        Debug.Log("Tiempo transcurrido: " + segundos + " segundos");*/
        // Asegúrate de incrementar finishedCoroutinesCount cuando cada Coroutine termine
        finishedCoroutinesCount++;

        // Si todas las Coroutines han terminado, detén el Stopwatch
        if (finishedCoroutinesCount >= coroutinesCount)
        {
            stopwatch.Stop();
            double segundos = stopwatch.Elapsed.TotalSeconds;
            Debug.Log("Tiempo transcurrido: " + segundos + " segundos");
        }
    }
    void LoadModel(string modelPath, int parent)
    {
        // Obtén todos los archivos .obj en el directorio
        string[] objFiles = System.IO.Directory.GetFiles(modelPath, "*.obj");
        
        string[] mtlFiles = System.IO.Directory.GetFiles(modelPath, "*.mtl");
        Debug.Log(mtlFiles[0]);

        if(objFiles.Length == 0)
        {
            Debug.LogError("No se encontró ningún archivo .obj en " + modelPath);
            return;
        }
    
        if(objFiles.Length > 1)
        {
            Debug.LogWarning("Se encontraron múltiples archivos .obj en " + modelPath + ". Cargando el primero.");
        }
        ResetWrapper();
        GameObject model = new OBJLoader().Load(objFiles[0], mtlFiles[0]);
        model.transform.SetParent(roots[parent].transform);

        //Take the loaded model to the placeholder position
        model.transform.localPosition = new Vector3(0, 0, 0);

        // Calcula el punto más bajo del objeto cargado.
        float puntoMasBajo = GetPuntoMasBajo(model);

        // Calcula la diferencia en Y.
        float diferenciaY = roots[parent].transform.position.y - puntoMasBajo;

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

    void Descomprimir(string zipPath, string extractPath)
    {
        ZipFile.ExtractToDirectory(zipPath, extractPath);
    }
    
}
