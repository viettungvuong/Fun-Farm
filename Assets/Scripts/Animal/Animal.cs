// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [CreateAssetMenu]
// public class Animal : ScriptableObject
// {
//     public GameObject prefab;
//     public int dailyMoney;
//     public int sellMoney;
//     public int buyMoney;
// }

// public class UsingAnimal{
//     public static List<UsingAnimal> animals = new List<UsingAnimal>();

//     [SerializeField] private Animal animal;

//     public UsingAnimal(){

//     }

//     public UsingAnimal(Animal animal){
//         this.animal = animal;
//     }

//     public void SetAnimal(Animal animal){
//         this.animal = animal;
//     }

//     void SellAnimal(PlayerUnit unit){
//         unit.AddMoney(animal.sellMoney);
//     }

//     bool BuyAnimal(PlayerUnit unit){
//         if (unit.SufficientMoney(animal.buyMoney)){
//             unit.UseMoney(animal.buyMoney);
//             return true;
//         }
//         else{
//             return false;
//         }
//     }


// }