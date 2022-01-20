using UnityEngine;
/// <summary>
/// Unused class. Ignore for now. Originally to test mesh and boundingbox positions and size.
/// </summary>
public class PlaceThreeTanksAtStart : MonoBehaviour
{
    // public Tank tankToPlace;
    public GameObject tankPrefab;
    private GameObject tankInstance;
    public GameObject fishPrefab;
    
    [HideInInspector]
    public int index;

    void Start()
    {
        /*for (int i = 0; i < 3; i++)
        {
            PlaceObject(i);
        }
        PrintIDs();*/
        
        PlaceObject(0);
        AddFish();
    }

    /**
     * Fish spawned in Random position im Tank, integrate woop woop 
     */
    private void AddFish()
    {
        Vector3 pos = tankInstance.GetComponent<MeshFilter>().mesh.bounds.center +
                      new Vector3(
                          Random.Range(-tankInstance.GetComponent<MeshFilter>().mesh.bounds.size.x / 2 + fishPrefab.GetComponent<Renderer>().bounds.extents.x,
                              tankInstance.GetComponent<MeshFilter>().mesh.bounds.size.x / 2 - fishPrefab.GetComponent<Renderer>().bounds.extents.x),
                          Random.Range(-tankInstance.GetComponent<MeshFilter>().mesh.bounds.size.y / 2 + fishPrefab.GetComponent<Renderer>().bounds.extents.y,
                              tankInstance.GetComponent<MeshFilter>().mesh.bounds.size.y / 2 - fishPrefab.GetComponent<Renderer>().bounds.extents.y),
                          Random.Range(-tankInstance.GetComponent<MeshFilter>().mesh.bounds.size.z / 2 + fishPrefab.GetComponent<Renderer>().bounds.extents.z,
                              tankInstance.GetComponent<MeshFilter>().mesh.bounds.size.z / 2 - fishPrefab.GetComponent<Renderer>().bounds.extents.z));
        Instantiate(fishPrefab, pos, Quaternion.identity, transform);
        
    }
    

    private void PrintIDs()
    {
        foreach (Tank tank in FindObjectsOfType<Tank>())
        {
            Debug.Log(tank.GetID());
        }
    }

    public void PlaceObject(int number)
    {
        // Tank tank = _gameObject.AddComponent<Tank>();
        // tank.name = "Tank_GO";
        // tank.SetID(index);
        // index++;
        // _gameObject = new GameObject();
        tankInstance = Instantiate(tankPrefab, new Vector3(number, 0, 0), Quaternion.identity);
        tankInstance.GetComponent<Tank>().SetID(index);
        index++;
        /*
         * Über GetComponent<MeshFilter>().mesh.bounds.size.x/y/z kommen wir an die genauen Maße der Aquarien
         */
        /*Debug.Log(Tank.GetComponent<MeshFilter>().mesh.bounds.size.x);
        Debug.Log(Tank.GetComponent<MeshFilter>().mesh.bounds.size.y);
        Debug.Log(Tank.GetComponent<MeshFilter>().mesh.bounds.size.z);*/
        /*
         * Meshfilter gibt uns den lokalen Mittelpunkt
         * Renderer bzw. MeshRenderer geben den globalen
         */
        // Debug.Log(Tank.GetComponent<MeshFilter>().mesh.bounds.extents);
        // Debug.Log(tankInstance.GetComponent<MeshFilter>().mesh.bounds.center);
        // Debug.Log(tankInstance.GetComponent<MeshRenderer>().bounds.center);
        Debug.Log(tankInstance.gameObject.layer);

        //Instantiate(tank, new Vector3(number, 0, number), Quaternion.identity);
        // tanks.Add(tank);

        // Instantiate(tankToPlace, new Vector3(number, 0,number), Quaternion.Euler(0,0,0));
        // tankToPlace.SetID(index);
        // index++;
        // tanks.Add(tankToPlace);
    }
}
