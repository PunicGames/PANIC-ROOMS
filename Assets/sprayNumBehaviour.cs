using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sprayNumBehaviour : MonoBehaviour
{
    [SerializeField] List<Texture2D> m_numbers;
    [SerializeField] Material m_material;
    static int m_currentNumber;
    private int m_current;


    // Start is called before the first frame update
    void Start()
    {
        //Ask to game manager
        m_currentNumber = 9;
        m_material.SetTexture("_Albedo", m_numbers[m_currentNumber]);
        m_material.SetTexture("_Opacity", m_numbers[m_currentNumber]);
    }

    private void Update()
    {
        if(m_current != m_currentNumber)
        {
            m_current = m_currentNumber;
            m_material.SetTexture("_Albedo", m_numbers[m_currentNumber]);
            m_material.SetTexture("_Opacity", m_numbers[m_currentNumber]);
        }
    }

    public static void updateNumbersLeft(int number)
    {
        m_currentNumber = number;
    }
}
