using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
	private DatabaseReference reference;

	private void Awake()
	{
		FirebaseApp app = FirebaseApp.DefaultInstance;
		reference = FirebaseDatabase.DefaultInstance.RootReference;
	}

	private void Start()
	{
		WriteDatabase("123", "Xin chao the gioi");
		ReadDatabase("123");
	}

	public void WriteDatabase(string id, string message)
	{
		reference.Child("User").Child(id).SetValueAsync(message).ContinueWithOnMainThread(task =>
		{
			if (task.IsCompleted)
			{
				Debug.Log("Ghi du lieu thanh cong");
			}
			else
			{
				Debug.Log("Ghi du lieu that bai" + task.Exception);
			}
		});
	}

	public void ReadDatabase(string id)
	{
		reference.Child("User").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
		{
			if (task.IsCompleted)
			{
				DataSnapshot snapshot = task.Result;
				Debug.Log("Doc du lieu thanh cong: " + snapshot.Value.ToString());
			}
			else
			{
				Debug.Log("Doc du lieu that bai" + task.Exception);
			}
		});
	}
}
