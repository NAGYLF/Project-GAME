using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using ItemHandler;

namespace Assets.Scripts
{ 
    public class ItemSlot : MonoBehaviour
    {
        #region Set In Inspector
        public bool IsEquipment = false;//csak az inspectorban allithato be
        public string SlotType;//azon tipusok melyeket befogadhat, ha nincs megadva akkor mindent.
        #endregion

        [HideInInspector] public Item SlotParentItem;
        [HideInInspector] public int sectorId;
        [HideInInspector] public (int Height,int Width) Coordinate;

        #region  Runtime Instantiated Objects Datas
        public GameObject PartOfItemObject;//ezen értéket egy itemslot egy item vizualizációjakor kellene hogy kapjon
        public GameObject ActualPartOfItemObject;//ezt vizualizációkor kapja és továbbiakban a vizualizációban lesz fumciója az iteomobjectum azonosításban
        public bool MouseOver = false;

        public Color color;
        public GameObject Title;
        public Image Background;
        #endregion
        public void Deactivation()//3
        {
            Background.color = color;
        }
        public void Open()//2
        {
            Background.color = Color.yellow;
        }
        public void Close()//2
        {
            Background.color = Color.red;
        }
        private void OnMouseEnter()
        {
            MouseOver = true;
        }
        private void OnMouseExit()
        {
            MouseOver = false;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            ActualPartOfItemObject = collision.gameObject;
            SlotParentItem.ContainerObject.GetComponent<ContainerObject>().activeSlots.Add(gameObject);
            SlotParentItem.ContainerObject.GetComponent<ContainerObject>().ChangedFlag = true;
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            SlotParentItem.ContainerObject.GetComponent<ContainerObject>().activeSlots.Remove(gameObject);
            ActualPartOfItemObject = null;
            SlotParentItem.ContainerObject.GetComponent<ContainerObject>().ChangedFlag = true;
        }
        private void Awake()
        {
            if (!IsEquipment)
            {
                Title.GetComponent<TextMeshPro>().text = SlotType;
            }
            color = Background.color;
        }
    }
}