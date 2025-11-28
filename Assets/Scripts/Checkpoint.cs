using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour 
{
	public int checkpointIndex;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			RaceManager.Instance.CheckpointReached(checkpointIndex);
		}
	}
}
