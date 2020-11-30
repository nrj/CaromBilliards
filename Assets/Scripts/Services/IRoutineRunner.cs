// Interface for a service.
// In this case, the service allows us to run a Coroutine outside
// the strict confines of a MonoBehaviour. (See RoutineRunner)
//
// https://github.com/strangeioc/strangerocks/blob/master/StrangeRocks/Assets/scripts/strangerocks/common/util/IRoutineRunner.cs

using System;
using UnityEngine;
using System.Collections;

public interface IRoutineRunner
{
    Coroutine StartCoroutine(IEnumerator method);
}