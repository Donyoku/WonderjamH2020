﻿using Rewired;
using UnityEngine;
using System.Collections;
using UI.ChoicePopup;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 2;
    [SerializeField]
    private int playerID = 0;

    private bool inMenu;
    private bool inQTE;

    private Rewired.Player inputManager;
    private Vector2 input, direction = Vector2.down; // direction will be used for animations
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        inputManager = ReInput.players.GetPlayer(playerID);
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        input = new Vector2(inputManager.GetAxis("Horizontal"), inputManager.GetAxis("Vertical"));
        if (input.magnitude < .1f) input = Vector2.zero;

        if( input != Vector2.zero)
        {
            direction = input.normalized;
        }
    }

    private void FixedUpdate()
    {
        if(!inQTE && !inMenu)
        {
            _rigidbody2D.MovePosition(_rigidbody2D.position + speed * input * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ChoicePopup choicePopUp = collision.GetComponent<ChoicePopup>();
        if(choicePopUp != null)
        {
            StartCoroutine(DisplayPopUp(choicePopUp));
        }
    }

    private IEnumerator DisplayPopUp(ChoicePopup choicePopUp)
    {
        choicePopUp.Display(transform.position);
        inMenu = true;
        bool choiceMade = false;
        while(!choiceMade)
        {
            if (inputManager.GetButtonDown("Cancel"))
            {
                Debug.Log("Cancel");
                choiceMade = true;
                break;
            }
            float input = inputManager.GetAxis("Horizontal");
            if(inputManager.GetButtonDown("MenuLeft"))
            {
                Debug.Log("Gauche");
                choicePopUp.GoLeft();
            }
            if(inputManager.GetButtonDown("MenuRight"))
            {
                Debug.Log("Droite");
                choicePopUp.GoRight();
            }
            if (inputManager.GetButton("Validate"))
            {
                Debug.Log("Validé !");
                choiceMade = true;
                choicePopUp.Validate();
            }
            yield return true;
        }
        inMenu = false;
        choicePopUp.Hide();
    }
}
