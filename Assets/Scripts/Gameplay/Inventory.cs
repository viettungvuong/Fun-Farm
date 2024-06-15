using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // create a scriptable object called item
    private List<ItemUnit> items;
    // Start is called before the first frame update
    void Start()
    {
        items = new List<ItemUnit>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddItem(ItemUnit item){
        items.Add(item);
    }

    void TakeOne(ItemUnit item){
        if (item.quantity>0){
            item.quantity--;
        }
    }
}
