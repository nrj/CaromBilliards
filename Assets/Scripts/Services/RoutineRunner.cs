// This is a common service/model pattern in Strange:
// We want something usually reserved to MonoBehaviours to be available
// elsewhere. Maybe someday we'll write a version that
// eschews MonoBehaviours altogether...but for now we simply leverage
// that behavior and provide it in injectable form.
//
// In this case, we're making Coroutines available everywhere in the app
// by attaching a MonoBehaviour to the ContextView.
//
// IRoutineRunner can be injected anywhere, minimizing direct dependency
// on MonoBehaviours.
//
// https://github.com/strangeioc/strangerocks/blob/master/StrangeRocks/Assets/scripts/strangerocks/common/util/RoutineRunner.cs

using System;
using strange.extensions.context.api;
using UnityEngine;
using System.Collections;
using strange.extensions.injector.api;

public class RoutineRunner : IRoutineRunner
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject contextView { get; set; }

    private RoutineRunnerBehaviour behaviour;

    [PostConstruct]
    public void PostConstruct()
    {
        behaviour = contextView.AddComponent<RoutineRunnerBehaviour>();
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return behaviour.StartCoroutine(routine);
    }
}

public class RoutineRunnerBehaviour : MonoBehaviour
{
}