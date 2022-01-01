using UnityEngine;

public class Tank : MonoBehaviour
{
    public int ID { get; set; }

    public int GetID()
    {
        return ID;
    }

    public void SetID(int id)
    {
        ID = id;
    }

    public Tank()
    {
        
    }
    
    public Tank(int id)
    {
        ID = id;
    }
}
