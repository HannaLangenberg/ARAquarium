using System;
using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects.Inventory.Scripts;
using Scriptable_Objects.Items.Scripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Display
{
    public class PlaceInScene : MonoBehaviour
    {
        public GameObject player;
        public GameObject placeholderTankEmpty;
        public GameObject fail;

        public TankObject activeTankObject;
        private GameObject _tankInstance, _fishInstance, _plantInstance, _decorInstance;
        // private Vector3 _fishVector3;
        private Dictionary<string, Vector3> _map;
        private List<GameObject> _fishInScene, _plantsInScene, _decorInScene;
        private Quaternion _rotation;
        private Vector3 _centerPoint;
        private float _maxSpawnRadius;
        private float _innerCircle;

        private float radius1, radius11, radius2, radius22;

        void Start()
        {
            _map = new Dictionary<string, Vector3>();
            _fishInScene = new List<GameObject>();
            _plantsInScene = new List<GameObject>();
            _decorInScene = new List<GameObject>();
            InitializeSelectedTank();
            InitializeAllPlants();
            InitializeAllDecor();
            InitializeAllFish();
        }

        //__AT START → Initialize functions_____________________________________________________________________________
        private void InitializeSelectedTank()
        {
            foreach (InventorySlot inventorySlot in player.GetComponent<Player>().tankInventory.container)
            {
                if (inventorySlot.amount == 1)
                {
                    Vector3 position = new Vector3(
                        -0.2f,
                        inventorySlot.item.prefab.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.y/2,
                        0);
                    if(inventorySlot.item.prefab.name.Equals("R_240L_120cm_Prefab"))
                    {
                        position += new Vector3(0, 0, -1f);
                    }
                    else if (inventorySlot.item.prefab.name.Equals("R_640L_180cm_Prefab"))
                    {
                        position += new Vector3(0, 0.05f, -2.2f);
                    }
                     
                    
                    _tankInstance = Instantiate(inventorySlot.item.prefab,
                        placeholderTankEmpty.transform.position
                        - position,
                        Quaternion.identity,
                        placeholderTankEmpty.transform);
                    activeTankObject = inventorySlot.item as TankObject;
                }
            }
        }
        private void InitializeAllFish() 
        { // return from insideTank is center, radius is activeTankObject.maxSpawnRadius
            // _fishVector3 = InsideTank();
            foreach (InventorySlot inventorySlot in player.GetComponent<Player>().fishInventory.container)
            {
                if (inventorySlot.amount > 0 && FishOkToAdd(inventorySlot))
                {
                    _map.Add(inventorySlot.item.tag, InsideTank(inventorySlot));
                    int fails = 0;
                    for (int i = 0; i < inventorySlot.amount; i++)
                    {
                        if (!PositionFish(inventorySlot))
                        {
                            fails++;
                        }
                    }
                    inventorySlot.amount -= fails;
                }
            }
        }
        private void InitializeAllPlants()
        {
            foreach (InventorySlot inventorySlot in player.GetComponent<Player>().plantInventory.container)
            {
                int fails = 0;
                if (inventorySlot.amount > 0)
                {
                    for (int i = 0; i < inventorySlot.amount; i++)
                    {
                        if (!PositionOther(inventorySlot))
                        {
                            fails++;
                        }
                    }
                }
                inventorySlot.amount -= fails;
            }
        }
        private void InitializeAllDecor()
        {
            foreach (InventorySlot inventorySlot in player.GetComponent<Player>().decorInventory.container)
            {
                int fails = 0;
                if (inventorySlot.amount > 0)
                {
                    for (int i = 0; i < inventorySlot.amount; i++)
                    {
                        if (!PositionOther(inventorySlot))
                        {
                            fails++;
                        }
                    }
                }
                inventorySlot.amount -= fails;
            }
        }
        
        //__AT RUNTIME → Add And remove_________________________________________________________________________________
        public bool AddNewOther(InventorySlot inventorySlot)
        {
            if (inventorySlot.item.type.Equals(ItemType.Fish))
            {
                if (FishOkToAdd(inventorySlot))
                {
                    CheckIfDictionaryContainsFish(inventorySlot);
                    return PositionFish(inventorySlot);
                }
            }
            else if (inventorySlot.item.type.Equals(ItemType.Plant) || inventorySlot.item.type.Equals(ItemType.Decor))
            {
                return PositionOther(inventorySlot);
            }

            return false;
        }

        private void CheckIfDictionaryContainsFish(InventorySlot inventorySlot)
        {
            if(!_map.ContainsKey(inventorySlot.item.tag))
                _map.Add(inventorySlot.item.tag, InsideTank(inventorySlot));

        }

        public void RemoveExistingOther(InventorySlot inventorySlot)
        {
            if (inventorySlot.item.type.Equals(ItemType.Fish))
            {
                _fishInScene.Reverse();
                foreach (GameObject fishInstance in _fishInScene)
                {
                    if (fishInstance.tag.Equals(inventorySlot.item.tag))
                    {
                        _fishInScene.Remove(fishInstance);
                        Destroy(fishInstance);
                        break;
                    }
                }
                _fishInScene.Reverse();
            }
            else if (inventorySlot.item.type.Equals(ItemType.Plant))
            {
                foreach (GameObject plantInstance in _plantsInScene)
                {
                    if (plantInstance.tag.Equals(inventorySlot.item.tag))
                    {
                        _plantsInScene.Remove(plantInstance);
                        Destroy(plantInstance);
                        break;
                    }
                }
            }
            else if (inventorySlot.item.type.Equals(ItemType.Decor))
            {
                foreach (GameObject decorInstance in _decorInScene)
                {
                    if (decorInstance.tag.Equals(inventorySlot.item.tag))
                    {
                        _decorInScene.Remove(decorInstance);
                        Destroy(decorInstance);
                        break;
                    }
                }
            }
        }
        public void SwitchTank()
        {
            Destroy(_tankInstance);
            activeTankObject = null;
            foreach (GameObject fishGO in _fishInScene)
            {
                Destroy(fishGO);
            }

            _map.Clear();
            _fishInScene.Clear();
            _plantsInScene.Clear();
            InitializeSelectedTank();
            InitializeAllPlants();
            InitializeAllDecor();
            InitializeAllFish();
        }
        
        //__Checks______________________________________________________________________________________________________
        private Vector3 InsideTank(InventorySlot inventorySlot)
        {
            Bounds tankBounds = _tankInstance.GetComponent<MeshRenderer>().bounds;
            float greatestExtend = CalculateGreatestExtend(inventorySlot) * 4;
            if (inventorySlot.amount == 0)
            {
                _maxSpawnRadius = greatestExtend * 4;
                _innerCircle = greatestExtend * 2;
                
            }
            else
            {
                _maxSpawnRadius = greatestExtend * (inventorySlot.amount + 2);
                _innerCircle = greatestExtend * inventorySlot.amount;
            }
            if (_maxSpawnRadius > activeTankObject.maxSpawnRadius)
            {
                _maxSpawnRadius = activeTankObject.maxSpawnRadius;
                _innerCircle    = _maxSpawnRadius - greatestExtend * 2;
            }
                    //FOR DEBUGGING
                    if (inventorySlot.item.tag.Equals("Green Neon Tetra"))
                    {
                        radius1 = _maxSpawnRadius;
                        radius11 = _innerCircle;
                    }
                    else
                    {
                        radius2 = _maxSpawnRadius;
                        radius22 = _innerCircle;
                    }
            
            return tankBounds.center +
                   new Vector3(
                       Random.Range(
                           -tankBounds.extents.x * 0.9f + _maxSpawnRadius,
                            tankBounds.extents.x * 0.9f - _maxSpawnRadius),
                       Random.Range( 
                           -tankBounds.extents.y * 0.9f + _maxSpawnRadius,
                            tankBounds.extents.y * 0.9f - _maxSpawnRadius),
                       Random.Range( 
                           -tankBounds.extents.z * 0.9f + _maxSpawnRadius,
                            tankBounds.extents.z * 0.9f - _maxSpawnRadius)
                   );
        }
        private bool FishOkToAdd(InventorySlot inventorySlot)
        {
            foreach (FishObject fishObject in activeTankObject.disabledFish)
            {
                if (inventorySlot.item.tag.Equals(fishObject.tag))
                {
                    return false;
                }
            }
            return true;
        }
        
        //__Calculations________________________________________________________________________________________________
        private bool PositionOther(InventorySlot inventorySlot)
        {
            bool successful;
            CalcPosAndRotAndScl(inventorySlot);
            int counter = 0;
            Collider[] results = {new Collider()};
            while (Physics.OverlapBoxNonAlloc(_centerPoint,
                       inventorySlot.item.prefab.GetComponent<MeshRenderer>().bounds.extents, results, _rotation) > 0
                   && counter < 30)
            {
                CalcPosAndRotAndScl(inventorySlot);
                counter++;
            }

            if (counter == 30)
            {
                Debug.Log(inventorySlot.item.tag + " konnte leider nicht hinzugefügt werden.");

                fail.SetActive(true);
                fail.GetComponent<Text>().text =
                    inventorySlot.item.tag + " konnte nicht hinzugefügt werden. Aquarium zu voll.";
                successful = false;
            }
            else
            {
                if (inventorySlot.item.type.Equals(ItemType.Plant))
                {
                    _plantInstance = Instantiate(inventorySlot.item.prefab, _centerPoint, _rotation,
                                                _tankInstance.transform);
                    _plantsInScene.Add(_plantInstance);
                    ResetRandomScale(inventorySlot);
                }
                else if (inventorySlot.item.type.Equals(ItemType.Decor))
                {
                    _decorInstance = Instantiate(inventorySlot.item.prefab, _centerPoint, _rotation,
                        _tankInstance.transform);
                    _decorInScene.Add(_decorInstance);
                    // ResetRandomScale(inventorySlot);
                }
                fail.SetActive(false);
                successful = true;
            }
            return successful;
        }
        private bool PositionFish(InventorySlot inventorySlot)
        {
            bool successful;
            ApplyRandomRotation(inventorySlot, 0);
            SpawnAroundParent(inventorySlot);
            int counter = 0;
            Collider[] results = {new Collider()};
            while (Physics.OverlapBoxNonAlloc(_centerPoint,
                       inventorySlot.item.prefab.GetComponent<MeshRenderer>().bounds.extents, results, _rotation) > 0
                   && counter < 30)
            {
                SpawnAroundParent(inventorySlot);
                counter++;
            }

            if (counter == 30)
            {
                Debug.Log(inventorySlot.item.tag + " konnte leider nicht hinzugefügt werden.");

                fail.SetActive(true);
                fail.GetComponent<Text>().text =
                    inventorySlot.item.tag + " konnte nicht hinzugefügt werden. Aquarium zu voll.";
                successful = false;
            }
            else
            {
                _fishInstance = Instantiate(inventorySlot.item.prefab, _centerPoint, _rotation,
                                            _tankInstance.transform);
                _fishInScene.Add(_fishInstance);
                fail.SetActive(false);
                successful = true;
            }
            return successful;
        }
        private float CalculateGreatestExtend(InventorySlot inventorySlot)
        {
            Vector3 extends = inventorySlot.item.prefab.GetComponent<MeshRenderer>().bounds.extents;
            return Mathf.Max(Mathf.Max(extends.x, extends.y), extends.z);
        }
        private void SpawnAroundParent(InventorySlot inventorySlot)
        {
            if (inventorySlot.amount > 0)
            {
                RecalculateSpawnRadii(inventorySlot);
            }
            Vector3 position = _map[inventorySlot.item.tag] + 
                               Random.insideUnitSphere * _innerCircle;
            _centerPoint = position;
        }
        private void RecalculateSpawnRadii(InventorySlot inventorySlot)
        {
            float greatestExtend = CalculateGreatestExtend(inventorySlot); 
            _maxSpawnRadius = greatestExtend * (inventorySlot.amount + 2);
            _innerCircle = greatestExtend * inventorySlot.amount;
            
            if (_maxSpawnRadius > activeTankObject.maxSpawnRadius)
            {
                _maxSpawnRadius = activeTankObject.maxSpawnRadius;
                _innerCircle    = _maxSpawnRadius - greatestExtend * 2;
            }
                if (inventorySlot.item.tag.Equals("Green Neon Tetra"))
                {
                    radius1 = _maxSpawnRadius;
                    radius11 = _innerCircle;
                }
                else
                {
                    radius2 = _maxSpawnRadius;
                    radius22 = _innerCircle;
                }
        }
        private void CalcPosAndRotAndScl(InventorySlot inventorySlot)
        {
            Bounds tankBounds = _tankInstance.GetComponent<MeshRenderer>().bounds;
            ApplyRandomRotation(inventorySlot, 1);
            // if (inventorySlot.item.type.Equals(ItemType.Plant) || inventorySlot.item.type.Equals(ItemType.Decor))
                ApplyRandomScale(inventorySlot);
            Bounds otherBounds = inventorySlot.item.prefab.GetComponent<MeshRenderer>().bounds;

            /*float y = 0;

            if (inventorySlot.item.type.Equals(ItemType.Fish))
            {
                y = Random.Range(
                    -tankBounds.extents.y * .8f + otherBounds.extents.y,
                    tankBounds.extents.y * .8f - otherBounds.extents.y);
            }
            else if (inventorySlot.item.type.Equals(ItemType.Plant) || inventorySlot.item.type.Equals(ItemType.Decor))
            {
               y = -tankBounds.extents.y * .9f;
            }*/

            _centerPoint = tankBounds.center +
                           new Vector3(
                               Random.Range(
                                   -tankBounds.extents.x * .8f + otherBounds.extents.x,
                                    tankBounds.extents.x * .8f - otherBounds.extents.x),
                               -tankBounds.extents.y * .9f,
                               // y,
                               Random.Range(
                                   -tankBounds.extents.z * .8f + otherBounds.extents.z,
                                    tankBounds.extents.z * .8f - otherBounds.extents.z)
                           );
        }
        
        //__Operations__________________________________________________________________________________________________
        private void ApplyRandomRotation(InventorySlot inventorySlot, int decision)
        {
            switch (decision)
            {
                case 0: //fish
                    _rotation = Quaternion.AngleAxis(Random.Range(-30, 30), Vector3.up);
                    break;
                case 1: //other
                    _rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up);
                    break;
                
            }
            inventorySlot.item.prefab.transform.rotation = _rotation;
        }
        private void ApplyRandomScale(InventorySlot inventorySlot)
        {
            float y = 0;

            if (inventorySlot.item.type.Equals(ItemType.Plant))
            {
                y = Random.Range(activeTankObject.decorScaleMin, activeTankObject.plantScaleMax);
            }
            else if (inventorySlot.item.type.Equals(ItemType.Decor))
            {
                y = Random.Range(activeTankObject.decorScaleMin, activeTankObject.decorScaleMax);
            }
            
            inventorySlot.item.prefab.transform.localScale =
            new Vector3(Random.Range(activeTankObject.decorScaleMin, activeTankObject.decorScaleMax),
                        y,
                        Random.Range(activeTankObject.decorScaleMin, activeTankObject.decorScaleMax));
        }
        private void ResetRandomScale(InventorySlot inventorySlot)
        {
            inventorySlot.item.prefab.transform.localScale =
            new Vector3(1,1,1);
        }
        
        //__DEBUG_______________________________________________________________________________________________________
        /*private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_map[_fishInScene[0].tag], radius1);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_map[_fishInScene[0].tag], radius11);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_map[_fishInScene[_fishInScene.Count-1].tag], radius2);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_map[_fishInScene[_fishInScene.Count-1].tag], radius22);
        }*/
    }
}