using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects.Inventory.Scripts;
using Scriptable_Objects.Items.Scripts;
using UI;
using Unity.Mathematics;
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
        //_____PUBLIC___________________________________________________________________________________________________
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
        /// Image <c>error</c>. The image which displays the error message "... could not be added".
        /// </summary>
        public Image error;
        /// <summary>
        /// Image <c>diskusWetterschmerle</c> displays a popup information that discus and weatherloach
        /// should not be placed in the same aquarium. 
        /// </summary>
        public Image diskusWetterschmerle;
        /// <summary>
        /// List <c>errorMessages</c>. A list that holds all errorMessages that could occur during placement.
        /// </summary>
        public List<Sprite> errorMessages;
        /// <summary>
        /// List <c>informationMessages</c>. A list that holds all informationMessages that could occur during placement.
        /// </summary>
        public List<Sprite> informationMessages;
        /// <summary>
        /// TankObject <c>activeTankObject</c>. A ScriptableObject TankObject which stores information about the selected aquarium. 
        /// </summary>
        public TankObject activeTankObject;
        /// <summary>
        /// InventorySlot <c>activeTankInventorySlot</c>. The currently selected InventorySlot for the aquarium. 
        /// </summary>
        public InventorySlot activeTankInventorySlot;
        //_____PRIVATE__________________________________________________________________________________________________
        /// <summary>
        /// GameObject <c>_...Instance</c>. Temporarily stores the current instance for further manipulation.
        /// </summary>
        private GameObject _tankInstance, _fishInstance, _plantInstance, _decorInstance;
        /// <summary>
        /// Dictionary <c>_swarmCenterPoints</c>. Stores the centerSpawnPoints for each fish species with the fish tag as the key.
        /// </summary>
        private Dictionary<string, Vector3> _schoolingCenterPoints;
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
            _schoolingCenterPoints = new Dictionary<string, Vector3>();
            _fishInScene = new List<GameObject>();
            _plantsInScene = new List<GameObject>();
            _decorInScene = new List<GameObject>();
            InitializeSelectedTank();
            InitializeAllPlants();
            InitializeAllDecor();
            InitializeAllFish();
        }

        //__AT START ??? Initialize functions_____________________________________________________________________________
        /// <summary>
        /// Initializes the selected aquarium. Only one aquarium at a time can have an amount of 1,
        /// all the others have an amount of 0. This is used to retrieve the correct selected aquarium.
        /// It is instantiated with respect to the UI elements in the center of the visible area.
        /// Afterwards the corresponding TankObject and inventorySlot are saved as the activeTankObject
        /// and activeTankInventorySlot so that its stats are available.
        /// </summary>
        private void InitializeSelectedTank()
        {
            foreach (var inventorySlot in player.GetComponent<Player>().tankInventory.container)
            {
                if (inventorySlot.amount != 1) continue;
                
                _tankInstance = Instantiate(
                    inventorySlot.item.prefab,
                    placeholderTankEmpty.transform.position - CalculatePositionInFrame(inventorySlot),
                    Quaternion.identity,
                    placeholderTankEmpty.transform);
                activeTankObject = inventorySlot.item as TankObject;
                activeTankInventorySlot = inventorySlot;
            }
        }

        /// <summary>
        /// This function initializes all fish.
        /// The fish is only initialized if its amount is greater than zero and is not disabled by the size of the
        /// aquarium (each has a list with disabledFish according to the available space).
        /// Fish of the same species spawn in a swarm. In the function InsideTank the spawn center is calculated.
        /// (For more information see this function)
        /// The spawnCenter is stored in a dictionary with the fish's tag as the key.
        /// There is a counter "fails". It is counted up if the fish could not be placed due to missing space.
        /// Fails is used to correct the fish's amount to show accurately how many are placed in the aquarium.
        /// </summary>
        private void InitializeAllFish() 
        {
            foreach (var inventorySlot in player.GetComponent<Player>().fishInventory.container)
            {
                if (inventorySlot.amount <= 0 || !FishOkToAdd(inventorySlot)) continue;
                
                _schoolingCenterPoints.Add(inventorySlot.item.tag, InsideTank(inventorySlot));
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
        /// This function initializes all plants with an amount greater than zero.
        /// The counter "fails" works just like the one for fish.
        /// </summary>
        private void InitializeAllPlants()
        {
            foreach (var inventorySlot in player.GetComponent<Player>().plantInventory.container)
            {
                var fails = 0;
                if (inventorySlot.amount <= 0) continue;
                
                for (var i = 0; i < inventorySlot.amount; i++)
                {
                    if (!PositionOther(inventorySlot))
                    {
                        fails++;
                    }
                }
                inventorySlot.amount -= fails;
            }
        }
        /// <summary>
        /// This function initializes all decor items with an amount greater than zero.
        /// The counter "fails" works just like the one for fish.
        /// </summary>
        private void InitializeAllDecor()
        {
            foreach (var inventorySlot in player.GetComponent<Player>().decorInventory.container)
            {
                var fails = 0;
                if (inventorySlot.amount <= 0) continue;
                
                for (var i = 0; i < inventorySlot.amount; i++)
                {
                    if (!PositionOther(inventorySlot))
                    {
                        fails++;
                    }   
                }
                
                inventorySlot.amount -= fails;
            }
        }
        
        //__AT RUNTIME ??? Add and Remove_________________________________________________________________________________
        /// <summary>
        /// This function is called whenever the user clicks plus on a fish, plant or decor item in the UI.
        /// A fish should only be added to the aquarium if it is not disabled. (Checked in FishOkToAdd)
        /// If the fish is allowed to be added it is checked if the dictionary already contains this fish species.
        /// Then PositionFish is called which tries to add the fish in the aquarium.
        /// If there the fish we are trying to place is a schooling fish and it is the first of its kind to be set,
        /// its minimum count is used and as many instances set.
        /// E.g. a discus should be placed in a group of at least 8 fish. But as it is quite large there might not be
        /// enough room for all 8 to spawn. So there is a counter which tracks how often the placing operation succedeed
        /// and sets the displays number accordingly.
        /// If the amount != 0, one fish is placed as usual.
        /// For plants or decor items PositionOther is called.
        /// </summary>
        /// <param name="inventorySlot">InventorySlot upon which add was called. Possible: fish, plant or decor.</param>
        /// <returns>
        /// true: if item was successfully added.
        /// false: if item could not be added.
        /// </returns>
        public bool AddNewOther(InventorySlot inventorySlot)
        {
            switch (inventorySlot.item.type)
            {
                case ItemType.Fish when !FishOkToAdd(inventorySlot):
                    return false;
                case ItemType.Fish:
                    CheckIfDictionaryContainsFish(inventorySlot);
                    bool success;
                    if (inventorySlot.amount == 0)
                    {
                        for (var i = 0; i < ((FishObject)inventorySlot.item).minCount; i++)
                        {
                            var added = 0;
                            if (PositionFish(inventorySlot))
                                added++;
                            player.GetComponent<Player>().fishInventory.AddOrRemoveAmount(inventorySlot, 0, added);
                        }
                        
                        error.gameObject.SetActive(true);
                        switch (inventorySlot.item.tag)
                        {
                            case "Discus":
                                error.sprite = informationMessages[1];
                                GetComponent<OtherSelection>().DisableFromFish(inventorySlot);
                                diskusWetterschmerle.gameObject.SetActive(true);
                                break;
                            case "Wetterschmerle":
                                error.sprite = informationMessages[0];
                                GetComponent<OtherSelection>().DisableFromFish(inventorySlot);
                                diskusWetterschmerle.gameObject.SetActive(true);
                                break;
                            case "Normal Neon Tetra":
                            case "Green Neon Tetra":
                            case "Blutsalmler":
                            case "Kirschfleckensalmler":
                            case "Rotaugensalmler":
                            case "Zitronensalmler":
                            case "Gl??hlichtsalmler":
                                error.sprite = informationMessages[2];
                                break;
                            case "L018 Golden Nugget Ancistrus":
                            case "L173 Zebra Ancistrus":
                            case "L142 Snowball Ancistrus":
                                error.gameObject.SetActive(false);
                                break;
                        }

                        StartCoroutine(HideErrorMessage(4f));
                        success = true;
                    }
                    else
                    {
                        if (PositionFish(inventorySlot))
                        {
                            player.GetComponent<Player>().fishInventory.AddOrRemoveAmount(inventorySlot, 0, 1);
                            success = true;
                        }
                        else
                            success = false;
                    }
                    return success;
                case ItemType.Plant:
                    if (PositionOther(inventorySlot))
                    {
                        player.GetComponent<Player>().plantInventory.AddOrRemoveAmount(inventorySlot, 0, 1);
                        return true;
                    }
                    else
                        return false;
                case ItemType.Decor:
                    if (PositionOther(inventorySlot))
                    {
                        player.GetComponent<Player>().decorInventory.AddOrRemoveAmount(inventorySlot, 0, 1);
                        return true;
                    }
                    else
                        return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// This function is called whenever a fish should be added to the aquarium.
        /// It checks if the fish in the inventorySlot is the first of its kind and does not have a
        /// _swarmCenterPoint yet or if a centerPoint is already known. If not a new _swarmCenterPoint is generated
        /// using the fish's tag as key and the return value of InsideTank as value. 
        /// </summary>
        /// <param name="inventorySlot">Stores which fish and how often it should be added.</param>
        private void CheckIfDictionaryContainsFish(InventorySlot inventorySlot)
        {
            if(!_schoolingCenterPoints.ContainsKey(inventorySlot.item.tag))
                _schoolingCenterPoints.Add(inventorySlot.item.tag, InsideTank(inventorySlot));
        }

        /// <summary>
        /// This function is called whenever the user clicks minus on a fish, plant or decor item in the UI.
        /// For fish, plant and decor item the operation is basically the same but other lists have to be used and the
        /// AddOrRemoveAmount function has to be executed on different inventories.
        /// Having found the correct one it is removed from the list and the instance is destroyed.
        /// For the fish, if we reach the minimum value or less all remaining instances are removed from the
        /// _fishInScene list, destroyed and the removed amount is subtracted.
        /// For the normal remove operation, the list which stores all the instances is reversed. This way we can remove
        /// the items backwards of how we placed them and not it the same order. So, the last item we placed will be
        /// removed first. We iterate through the list until we find an instance with a tag that equals the tag we are
        /// searching. Lastly we reverse the list again so that we won't run into problems when adding new items.
        /// For plants and decor items the same is done.
        /// </summary>
        /// <param name="inventorySlot">InventorySlot upon which remove was called. Possible: fish, plant or decor</param>
        public void RemoveExistingOther(InventorySlot inventorySlot)
        {
            switch (inventorySlot.item.type)
            {
                case ItemType.Fish:
                {
                    if (inventorySlot.amount <= ((FishObject)inventorySlot.item).minCount)
                    {
                        var fishInstances = GameObject.FindGameObjectsWithTag(inventorySlot.item.tag);
                        foreach (var instance in fishInstances)
                        {
                            _fishInScene.Remove(instance);
                            Destroy(instance);
                        }
                        
                        player.GetComponent<Player>().fishInventory.AddOrRemoveAmount(
                            inventorySlot,
                            1,
                            fishInstances.Length);
                    }
                    else
                    {
                        _fishInScene.Reverse();
                        foreach (var fishInstance in _fishInScene)
                        {
                            if (!fishInstance.tag.Equals(inventorySlot.item.tag)) continue;
                            
                            _fishInScene.Remove(fishInstance);
                            player.GetComponent<Player>().fishInventory.AddOrRemoveAmount(inventorySlot, 1, 1);
                            Destroy(fishInstance);
                            break;
                        }
                        _fishInScene.Reverse();
                    }
                    break;
                }
                case ItemType.Plant:
                {
                    _plantsInScene.Reverse();
                    foreach (var plantInstance in _plantsInScene)
                    {
                        if (!plantInstance.tag.Equals(inventorySlot.item.tag)) continue;
                        
                        _plantsInScene.Remove(plantInstance);
                        Destroy(plantInstance);
                        player.GetComponent<Player>().plantInventory.AddOrRemoveAmount(inventorySlot, 1, 1);
                        break;
                    }
                    _plantsInScene.Reverse();
                    break;
                }
                case ItemType.Decor:
                {
                    _decorInScene.Reverse();
                    foreach (var decorInstance in _decorInScene)
                    {
                        if (!decorInstance.tag.Equals(inventorySlot.item.tag)) continue;
                        
                        _decorInScene.Remove(decorInstance);
                        Destroy(decorInstance);
                        player.GetComponent<Player>().decorInventory.AddOrRemoveAmount(inventorySlot, 1, 1);
                        break;
                    }
                    _decorInScene.Reverse();
                    break;
                }
            }
        }
        /// <summary>
        /// Called whenever the user chooses a different aquarium.
        /// The old one is destroyed, destroying all its children with it.
        /// Afterwards everything is reset or cleared and then newly initialized. 
        /// </summary>
        public void SwitchTank()
        {
            Destroy(_tankInstance);
            activeTankObject = null;
            activeTankInventorySlot = null;
            _schoolingCenterPoints.Clear();
            _fishInScene.Clear();
            _plantsInScene.Clear();
            _decorInScene.Clear();
            InitializeSelectedTank();
            InitializeAllPlants();
            InitializeAllDecor();
            InitializeAllFish();
        }
        
        //__Checks______________________________________________________________________________________________________
        /// <summary>
        /// This function calculates the _swarmCenterPoint for the fish species.
        /// It is only called with a set amount. At the beginning with a given amount from the inventory or whenever a
        /// new fish is added with the amount 0.
        /// The _tankInstance's bounds are extracted. They are later used to calculate a point within their extends.
        /// The greatestExtend ("1/2 longest Side") of the fish is calculated in CalculateGreatestExtend and
        /// multiplied with 4 to have twice the longest side. The greatestExtend will be used to calculate the spwan
        /// radius in which the fish can be spawned.
        /// For the discus an extra case was construced as it is quite large and otherwise there might be problems when
        /// placing the discus due to missing space. So its spawncircle is larger than it would have to be.
        /// The idea is that the spawn radius is dependent on the amount of fish from that species. So for the first
        /// fish the radius could be quite small and for every added fish it will grow. The problem is if the
        /// centerPoint of the spawnCircle is set near the glass fronts. Then there would not be enough space for the
        /// new fish to spawn. So the spawnCircle is here calculated a bit larger than needed to push the center away
        /// from the aquarium's glass walls. 
        /// *4 is a safety measure to ensure that the fish will be entirely contained within the aquarium.
        /// If it is the first fish of this kind the _maxSpawnRadius will be greatestExtend *4 again and
        /// the _innerCircle will always be 2 times smaller than maximum.
        /// If the amount is greater than zero _maxSpawnRadius will be the greatestExtend * (amount +2) and the
        /// _innerCirlce greatestExtend * amount.
        /// When generating random points for the fish to spawn they will be inside _innerCircle. But as this point is
        /// the center of the fish, up to half a fish might stick out of the _innerCircle, so there is _maxSpawnRadius
        /// to ensure that there will always be a safety distance from the glass walls. 
        /// Lastly we have to make sure that the spawnCircles are not larger than the aquarium can handle. If they are
        /// they are set back to the maxSpawnRadius the aquarium allows and 2 times smaller.
        /// Returned will be a point within the bounds of the aquarium with respect to the _maxSpawnRadius.
        /// The tankBounds contain the entire aquarium but the area within the glass walls is 90% of that size.
        /// From the aquariums center the spawn range spreads from minus 90% of the extends plus _maxSpawnRadius to
        /// plus 90% of the extends minus _maxSpawnRadius.
        /// </summary>
        /// <param name="inventorySlot">Stores which fish and how often to add</param>
        /// <returns>A spawnCenterPoint for the fish species within the aquarium's bounds.</returns>
        private Vector3 InsideTank(InventorySlot inventorySlot)
        {
            var tankBounds = _tankInstance.GetComponent<MeshRenderer>().bounds;
            var greatestExtend = CalculateGreatestExtend(inventorySlot) * 4;
            if (inventorySlot.amount == 0)
            {
                _maxSpawnRadius = greatestExtend * 4;
                _innerCircle = greatestExtend * 2;
            }
            else if (inventorySlot.amount == 0 && inventorySlot.item.tag.Equals("Discus"))
            {
                _maxSpawnRadius = greatestExtend * (((FishObject)inventorySlot.item).minCount + 2);
                _innerCircle = greatestExtend * ((FishObject)inventorySlot.item).minCount;
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
        /// <summary>
        /// This function is called during initializeAllFish and adding operations.
        /// It checks whether this fish is allowed to be added by looping through the aquarium's disabled fish and
        /// comparing their tags.
        /// </summary>
        /// <param name="inventorySlot">Which fish should be added</param>
        /// <returns>
        /// true: if this fish is not on the disabled fish list.
        /// flase: if this fish is disabled by the aquarium.
        /// </returns>
        private bool FishOkToAdd(InventorySlot inventorySlot)
        {
            foreach (var fishObject in activeTankObject.disabledFish)
            {
                if (inventorySlot.item.tag.Equals(fishObject.tag))
                {
                    return false;
                }
            }
            return true;
        }
        
        //__Calculations________________________________________________________________________________________________
        /// <summary>
        /// This function is called during initializeAll plants and decor, and adding operations.
        /// It tries to position plants or decor items. First of all, the item is randomly positioned on the floor,
        /// rotated and scaled. Afterwards its collider is used to check if the item is placed without collisions.
        /// If so, the item is instantiated, added to the _...InScene list, the amount in the inventory is counted
        /// up, and the prefab's random scale is reset.
        /// On the other hand if a collision was detected and the counter is less than 30 a new position, rotation
        /// and scale is calculated. Once the counter reaches 30 a normally hidden error text is activated on the UI
        /// and informes the user that the item could not be added due to missing space.
        /// </summary>
        /// <param name="inventorySlot">Item to be positioned</param>
        /// <returns>
        /// true: if item could be added
        /// false: if item could not be added
        /// </returns>
        private bool PositionOther(InventorySlot inventorySlot)
        {
            bool successful;
            var counter = 0;
            CalcPRSOnTheFloor(inventorySlot);
            Collider[] results = {new Collider()};
            while (Physics.OverlapBoxNonAlloc(_centerPoint,
                       inventorySlot.item.prefab.GetComponent<MeshRenderer>().bounds.extents, results, _rotation) > 0
                   && counter < 30)
            {
                CalcPRSOnTheFloor(inventorySlot);
                counter++;
            }

            if (counter == 30)
            {
                Debug.Log(inventorySlot.item.tag + " konnte leider nicht hinzugef??gt werden.");

                error.sprite = inventorySlot.item.type.Equals(ItemType.Plant) ? errorMessages[1] : errorMessages[2];
                if (!error.gameObject.activeSelf)
                {
                    error.gameObject.SetActive(true);
                    StartCoroutine(HideErrorMessage(2f));
                }
                
                successful = false;
            }
            else
            {
                switch (inventorySlot.item.type)
                {
                    case ItemType.Plant:
                        _plantInstance = Instantiate(inventorySlot.item.prefab, _centerPoint, _rotation,
                            _tankInstance.transform);
                        _plantsInScene.Add(_plantInstance);
                        ResetRandomScaleAndRotation(inventorySlot);
                        break;
                    case ItemType.Decor:
                        _decorInstance = Instantiate(inventorySlot.item.prefab, _centerPoint, _rotation,
                            _tankInstance.transform);
                        _decorInScene.Add(_decorInstance);
                        ResetRandomScaleAndRotation(inventorySlot);
                        break;
                }

                if (error.gameObject.activeSelf)
                {
                    error.gameObject.SetActive(false);
                    StopCoroutine(HideErrorMessage(2f));
                }
                successful = true;
            }
            return successful;
        }

        private IEnumerator HideErrorMessage(float duration)
        {
            while (error.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(duration);
                error.gameObject.SetActive(false);
                if (diskusWetterschmerle.gameObject.activeSelf)
                {
                    diskusWetterschmerle.gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// This function is called during initializeAllFish and adding operations.
        /// It is similar to PositionOther the only differences being that the fish is not scaled and spawns around
        /// the species' centerPoint. Differentiates between ancistrus and other fish. Ancistrus does not swarm and
        /// swims on the ground so special handling is needed.
        /// </summary>
        /// <param name="inventorySlot">Fish to be positioned</param>
        /// <returns>
        /// true: if fish could be added
        /// false: if fish could not be added
        /// </returns>
        private bool PositionFish(InventorySlot inventorySlot)
        {
            bool successful;
            ApplyRandomRotation(inventorySlot);
            if (inventorySlot.item.tag.Contains("Ancistrus"))
            {
                CalcPRSOnTheFloor(inventorySlot);
            }
            else
            {
                SpawnAroundCenterPoint(inventorySlot);
            }
            var counter = 0;
            Collider[] results = {new Collider()};
            while (Physics.OverlapBoxNonAlloc(_centerPoint,
                       inventorySlot.item.prefab.GetComponent<MeshRenderer>().bounds.extents, results, _rotation) > 0
                   && counter < 30)
            {
                if (inventorySlot.item.tag.Contains("Ancistrus"))
                {
                    CalcPRSOnTheFloor(inventorySlot);
                }
                else
                {
                    SpawnAroundCenterPoint(inventorySlot);
                }
                counter++;
            }

            if (counter == 30)
            {
                Debug.Log(inventorySlot.item.tag + " konnte leider nicht hinzugef??gt werden.");

                error.sprite = errorMessages[0];
                if (!error.gameObject.activeSelf)
                {
                    error.gameObject.SetActive(true);
                    StartCoroutine(HideErrorMessage(2f));
                }
                
                successful = false;
            }
            else
            {
                _fishInstance = Instantiate(inventorySlot.item.prefab, _centerPoint, _rotation,
                                            _tankInstance.transform);
                _fishInScene.Add(_fishInstance);
                if (error.gameObject.activeSelf)
                {
                    error.gameObject.SetActive(false);
                    StopCoroutine(HideErrorMessage(2f));
                }
                successful = true;
            }
            return successful;
        }
        /// <summary>
        /// This function calculates the greatest extend of the fish prefab by comparing x, y and z using Mathf.Max.
        /// </summary>
        /// <param name="inventorySlot">Fish to be added</param>
        /// <returns>The greatest extend as a float value</returns>
        private float CalculateGreatestExtend(InventorySlot inventorySlot)
        {
            var extends = inventorySlot.item.prefab.GetComponent<MeshRenderer>().bounds.extents;
            return Mathf.Max(Mathf.Max(extends.x, extends.y), extends.z);
        }
        /// <summary>
        /// This function calculates the position with respect to the UI elements.
        /// For the two largest aquariums an additional correction has to be done.
        /// </summary>
        /// <param name="inventorySlot">Stores the selected aquarium.</param>
        /// <returns>A Vector3 storing the position for the aquarium.</returns>
        private Vector3 CalculatePositionInFrame(InventorySlot inventorySlot)
        {
            var position = new Vector3(
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
        /// This function generates a random point within the _innerCircle spawn radius. If necessary the spawnRadii
        /// are recalculated beforehand.
        /// </summary>
        /// <param name="inventorySlot">Fish to be added.</param>
        private void SpawnAroundCenterPoint(InventorySlot inventorySlot)
        {
            if (inventorySlot.amount > 0)
            {
                RecalculateSpawnRadii(inventorySlot);
            }
            var position = _schoolingCenterPoints[inventorySlot.item.tag] + 
                           Random.insideUnitSphere * _innerCircle;
            _centerPoint = position;
        }
        /// <summary>
        /// This function recalculates the spawnRadii to match the desired amount of fish.
        /// Checks that the radii do not outgrow the aquraium.
        /// </summary>
        /// <param name="inventorySlot">Fish to be added.</param>
        private void RecalculateSpawnRadii(InventorySlot inventorySlot)
        {
            var greatestExtend = CalculateGreatestExtend(inventorySlot); 
            _maxSpawnRadius = greatestExtend * (inventorySlot.amount + 2);
            _innerCircle = greatestExtend * inventorySlot.amount;

            if (!(_maxSpawnRadius > activeTankObject.maxSpawnRadius)) return;
            _maxSpawnRadius = activeTankObject.maxSpawnRadius;
            _innerCircle    = _maxSpawnRadius - greatestExtend * 2;
        }
        /// <summary>
        /// This function calls to apply a random rotation and scale to the plants and decor items.
        /// For the ancistrus only random rotation is applied.
        /// Sets the _centerPoint for the item respecting all bounds.
        /// </summary>
        /// <param name="inventorySlot">Item to be added.</param>
        private void CalcPRSOnTheFloor(InventorySlot inventorySlot)
        {
            var tankBounds = _tankInstance.GetComponent<MeshRenderer>().bounds;
            ApplyRandomRotation(inventorySlot);
            if (!inventorySlot.item.type.Equals(ItemType.Fish))
            {
                ApplyRandomScale(inventorySlot);
            }
            var otherBounds = inventorySlot.item.prefab.GetComponent<MeshRenderer>().bounds;

            _centerPoint = tankBounds.center +
                           new Vector3(
                               Random.Range(
                                   -tankBounds.extents.x * .8f + otherBounds.extents.x,
                                    tankBounds.extents.x * .8f - otherBounds.extents.x),
                               -tankBounds.extents.y * .9f,
                               Random.Range(
                                   -tankBounds.extents.z * .8f + otherBounds.extents.z,
                                    tankBounds.extents.z * .8f - otherBounds.extents.z)
                           );
        }
        
        //__Operations__________________________________________________________________________________________________
        /// <summary>
        /// This function applies a random or limited random rotation around the y axis to the items.
        /// For fish the rotation is limited to -30?? to 30??, for the rest a full 360?? is allowed.
        /// </summary>
        /// <param name="inventorySlot">Item to be placed.</param>
        private void ApplyRandomRotation(InventorySlot inventorySlot)
        {
            _rotation = Quaternion.AngleAxis(
                inventorySlot.item.type.Equals(ItemType.Fish)
                    ? Random.Range(-30, 30)
                    : Random.Range(-180, 180),
                                   Vector3.up);
            inventorySlot.item.prefab.transform.rotation = _rotation;
        }
        /// <summary>
        /// This function applies a limited random scale (depending on the aquarium's size) to the x and z dimensions
        /// and a type-specific scale factor to the y dimension.
        /// For the cave item in every rectangular aquarium, a scalefactor of 1.2f is used. 
        /// </summary>
        /// <param name="inventorySlot">Item to be displayed.</param>
        private void ApplyRandomScale(InventorySlot inventorySlot)
        {
            var y = inventorySlot.item.type switch
            {
                ItemType.Plant => Random.Range(activeTankObject.decorScaleMin, activeTankObject.plantScaleMax),
                ItemType.Decor => Random.Range(activeTankObject.decorScaleMin, activeTankObject.decorScaleMax),
                _ => 1
            };

            if (inventorySlot.item.tag.Equals("Cave") && activeTankObject.prefab.name.Contains("R"))
            {
                inventorySlot.item.prefab.transform.localScale =
                    new Vector3(1.2f, 1.2f, 1.2f);
            }
            else
            {
                inventorySlot.item.prefab.transform.localScale =
                            new Vector3(Random.Range(activeTankObject.decorScaleMin, activeTankObject.decorScaleMax),
                                        y,
                                        Random.Range(activeTankObject.decorScaleMin, activeTankObject.decorScaleMax));
            }
            
        }
        /// <summary>
        /// Resets the localScale of the prefab to (1,1,1) and the Rotation to (0,0,0).
        /// </summary>
        /// <param name="inventorySlot">Item to be reset.</param>
        private void ResetRandomScaleAndRotation(InventorySlot inventorySlot)
        {
            inventorySlot.item.prefab.transform.localScale =
            new Vector3(1,1,1);
            inventorySlot.item.prefab.transform.rotation = Quaternion.identity;
        }
    }
}