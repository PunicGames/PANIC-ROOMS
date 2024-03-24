using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayNumBehaviour : MonoBehaviour
{
    [SerializeField] List<Texture2D> m_numbers;
    [SerializeField] Material m_material;
    private int m_current;
   
    public void UpdateNumbersLeft(int number)
    {
        //Debug.Log(number);
        m_current = number;
        m_material.SetTexture("_Albedo", m_numbers[m_current]);
        m_material.SetTexture("_Opacity", m_numbers[m_current]);
    }
}
