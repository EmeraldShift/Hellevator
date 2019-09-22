using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
	public GameObject laser;

	bool isDamaging = false;
	Mittins mittins;

	void Update()
    {
		if (mittins.curLaserWarningTime > 0) isDamaging = false;
		else isDamaging = true;
    }
}
