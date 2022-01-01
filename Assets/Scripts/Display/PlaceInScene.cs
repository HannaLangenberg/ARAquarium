using System.Collections.Generic;
using Scriptable_Objects.Inventory.Scripts;
using Scriptable_Objects.Items.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Display
{
    /// <summary>
    /// Class <c>PlaceInScene</c> places all instances of selected prefabs in the scene.
    /// </summary>
    public class PlaceInScene : MonoBehaviour
    {
        /// <summary>
        /// GameObject <c>Player</c> for easier access to the scripts attached to player.
        /// </summary>
        public GameObject player;
        /// <summary>
        /// GameObject <c>placeholderTankEmpty</c> is the empty to which the selected aquarium will be
        /// parented. It itself is parented to the ARCamera.
        /// </summary>
        public GameObject placeholderTankEmpty;
        /// <summary>
        /// GameObject <c>fail</c>. The Textfield in wich the error message "... could not be added" will be displayed.
        /// </summary>
        public GameObject fail;
        /// <summary>
        /// TankObject <c>activeTankObject</c>. A ScriptableObject TankObject which stores information about the selected aquarium. 
        /// </summary>
        public TankObject activeTankObject;
        
        /// <summary>
        /// GameObject <c>_...Instance</c>. Temporarily stores the current instance for further manipulation.
        /// </summary>
        private GameObject _tankInstance, _fishInstance, _plantInstance, _decorInstance;
        /// <summary>
        /// Dictionary <c>_swarmCenterPoints</c>. Stores the centerSpawnPoints for each fish species with the fish tag as the key.
        /// </summary>
        private Dictionary<string, Vector3> _swarmCenterPoints;
        /// <summary>
        /// List <c>_...InScene</c>. Stores all instances of the specific type for add and remove operations. 
        /// </summary>
        private List<GameObject> _fishInScene, _plantsInScene, _decorInScene;
        /// <summary>
        /// Quaternion <c>_rotation</c>. The rotation of the fish, plant, decor instances.
        /// </summary>
        private Quaternion _rotation;
        /// <summary>
        /// Vector3 <c>_centerPoint</c>. Calculated centerPoint of the instance that should be spawned.
        /// </summary>
        private Vector3 _centerPoint;
        /// <summary>
        /// Float <c>_innerCircle</c>. The actual radius in which fish can be spawned around the _centerPoint without it
        /// sticking into the aquarium's walls.
        /// </summary>
        private float _innerCircle;
        /// <summary>
        /// Float <c>_maxSpawnRadius</c>. A safety additional radius around the _innerCircle to make sure the
        /// _centerPoint is far enough from the walls.
        /// </summary>
        private float _maxSpawnRadius;
        /// <summary>
        /// For more information see function: InsideTank
        /// </summary>

        /// <summary>
        /// Initializes the dictionary, lists and the aquarium, plants, decor items and fish.
        /// </summary>
        void Start()
        {
            _swarmCenterPoints = new Dictionary<string, Vector3>();
            _fishInScene = new List<GameObject>();
            _plantsInScene = new List<GameObject>();
            _decorInScene = new List<GameObject>();
            InitializeSelectedTank();
            InitializeAllPlants();
            InitializeAllDecor();
            InitializeAllFish();
        }

        //__AT START → Initialize functions_____________________________________________________________________________
        /// <summary>
        /// Initializes the selected aquarium. Only one aquarium at a time can have an amount of 1,
        /// all the others have an amount of 0. This is used to retrieve the correct selected aquarium.
        /// It is instantiated with respect to the UI elements in the center of the visible area.
        /// Afterwards the corresponding TankObject is saved as the activeTankObject so that its stats are available.
        /// </summary>
        private void InitializeSelectedTank()
        {
            foreach (InventorySlot inventorySlot in player.GetComponent<Player>().tankInventory.container)
            {
                if (inventorySlot.amount != 1) continue;
                
                _tankInstance = Instantiate(
                    inventorySlot.item.prefab,
                    placeholderTankEmpty.transform.position - CalculatePositionInFrame(inventorySlot),
                    Quaternion.identity,
                    placeholderTankEmpty.transform);
                activeTankObject = inventorySlot.item as TankObject;
            }
        }
        
        /// <summary>
        /// This function calculates the position with respect to the UI elements.
        /// For the two largest aquariums an additional correction has to be done.
        /// </summary>
        private Vector3 CalculatePositionInFrame(InventorySlot inventorySlot)
        {
            Vector3 position = new Vector3(
                -0.2f,
                inventorySlot.item.prefab.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.y/2,
                0);
            switch (inventorySlot.item.prefab.name)
            {
                case "R_240L_120cm_Prefab":
                    position += new Vector3(0, 0, -1f);
                    break;
                case "R_640L_180cm_Prefab":
                    position += new Vector3(0, 0.05f, -2.2f);
                    break;
            }

            return position;
        }

        /// <summary>
        /// This function initializes all fish.
        /// The fish is only initialized if its amount is greater than zero and is not disabled by the size of th
        /// aquarium (each has a list with disabledFish according to the available space).
        /// Fish of the same species spawn in a swarm. In the function InsideTank the spawn center is calculated.
        /// (For more information see this function)
        /// The spawnCenter is stored in a dictionary with the fish's tag as the key.
        /// There is a counter "fails". It is counted up if the fish could not be placed due to missing space.
        /// Fails is used to correct the fish's amount to show accurately how many are placed in the aquarium.
        /// </summary>
        private void InitializeAllFish() 
        {
            foreach (InventorySlot inventorySlot in player.GetComponent<Player>().fishInventory.container)
            {
                if (inventorySlot.amount <= 0 || !FishOkToAdd(inventorySlot)) continue;
                
                _swarmCenterPoints.Add(inventorySlot.item.tag, InsideTank(inventorySlot));
                var fails = 0;
                for (var i = 0; i < inventorySlot.amount; i++)
                {
                    if (!PositionFish(inventorySlot))
                    {
                        fails++;
                    }
                }
                inventorySlot.amount -= fails;
            }
        }
        
        /// <summary>
        /// This function initializes all plants.
        /// </summary>
        private void InitializeAllPlants()
        {
            foreach (InventorySlot inventorySlot in player.GetComponent<Player>().plantInventory.container)
            {
                var fails = 0;
                if (inventorySlot.amount > 0)
                {
                    for (var i = 0; i < inventorySlot.amount; i++)
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
            if(!_swarmCenterPoints.ContainsKey(inventorySlot.item.tag))
                _swarmCenterPoints.Add(inventorySlot.item.tag, InsideTank(inventorySlot));
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

            _swarmCenterPoints.Clear();
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
            Vector3 position = _swarmCenterPoints[inventorySlot.item.tag] + 
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
    }
}