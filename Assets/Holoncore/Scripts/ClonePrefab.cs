﻿using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEngine;

//
//Creation example tool to instantiate a cube in the network using PhotonNetwork.Instantiate
//The cube ownership is set to actor number when created, and to its color using SetColor RPC
//
namespace Networking.Pun2
{

    public class ClonePrefab : MonoBehaviourPun
    {
        [SerializeField] enum Hand { Right, Left };
        [SerializeField] Hand hand;
        private bool pressedLastFrame = false;
        private bool clonedLastFrame = false;
        private string myPrefabName;
        private Transform thisObjectTransform;
        [NonSerialized] public OVRGrabber grabber;
        
        //public GameObject prefabToClone;

        private void Start()
        {
            thisObjectTransform = gameObject.transform;
            myPrefabName = gameObject.name;
        }

        void Update()
        {
            
            float triggerAmount = 0;
            
            if (photonView.IsMine)
            {
                
                triggerAmount = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

                if (triggerAmount > 0.75f)
                {
                    pressedLastFrame = true;
                    
                    if (pressedLastFrame && !clonedLastFrame && grabber!=null)
                    {
                        Clone();
                        clonedLastFrame = true;
                    }
                }
                
                bool releasedTrigger = triggerAmount < 0.25f && pressedLastFrame;
                
                if(releasedTrigger)
                {
                    pressedLastFrame = false;
                    clonedLastFrame = false;

                }
                
            }
            
            void Clone()
            {
                GameObject obj = PhotonNetwork.Instantiate(myPrefabName, this.gameObject.transform.position, this.gameObject.transform.rotation, 0);
                //obj.transform.position = transform.position;
                //obj.transform.rotation = transform.localRotation;
                obj.transform.localScale = thisObjectTransform.transform.localScale;
                //obj.GetComponent<ClonePrefab>().prefabToClone = prefabToClone;
                obj.gameObject.name = myPrefabName;

                // Prevent cloned objects colliding with original.
                // This assumes that all gameobjects in the clonable's hierarchy are on the default layer
                // We undo this in Hand.DetachObject()
                // Hacky! We probably need to keep track of the entire "isTrigger" status for the cloned
                // objects hierarchy but that seems like a lot of effort.
                obj.layer = LayerMask.NameToLayer("Interactable");
                foreach (var go in obj.GetComponentsInChildren<Transform>())
                {
                    go.gameObject.layer = LayerMask.NameToLayer("Interactable");
                }

                //clone.transform.name = gameObject.name;
                //hand.AttachObject(clone, hand.GetBestGrabbingType(GrabTypes.None), Hand.AttachmentFlags.ParentToHand);
            }
               
        }

    }
}
