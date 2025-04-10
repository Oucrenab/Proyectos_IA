using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarlosLogica : MonoBehaviour
{
    [SerializeField] CarlosBoton[] _buttons;
    [SerializeField] List<CarlosBoton> _buttonsList;

    [SerializeField] int _round;
    [SerializeField] float _timeBtwButtons;

    bool trollButton = false;

    public delegate void VoidDelegate();
    VoidDelegate DetectPlayerInput;


    void Start()
    {
        foreach(var button in _buttons)
        {
            button.OnButtonActive += ButtonActive;
        }
    }

    IEnumerator ShowOrder()
    {
        for (int i = 0; i < _buttonsList.Count; i++)
        {


            yield return _timeBtwButtons;
        }
    }

    void AddButton()
    {
        var index = Random.Range(0, _buttons.Length);


        if (trollButton)
        {

        }
        else
        {
            _buttonsList.Add(_buttons[index]);

        }
    }

    void RoundComplete()
    {

    }

    void ButtonActive()
    {

    }

    void Loose()
    {

    }
}
