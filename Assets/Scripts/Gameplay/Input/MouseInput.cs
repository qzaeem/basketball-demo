using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Basketball_Demo.Gameplay
{
    public class MouseInput : InputBase
    {
        const float dragThreshold = 5;
        const float dragEndDuration = 0.15f;
        const float inputRefreshDuration = 1;

        protected override async UniTask ScanInput()
        {
            CancellationToken token = TaskUtils.RenewCTS(ref inputCTS);

            while (true)
            {
                await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);

                Vector2 startPosition = Input.mousePosition;

                await UniTask.WaitUntil(() =>
                    Input.GetMouseButtonUp(0) || Vector2.Distance(Input.mousePosition, startPosition) > dragThreshold, cancellationToken: token);

                if (!Input.GetMouseButton(0))
                {
                    continue;
                }

                OnInputStarted(Input.mousePosition);
                float elapsedDragDuration = dragEndDuration;
                Vector2 lastMousePosition = Input.mousePosition;

                while (Input.GetMouseButton(0))
                {
                    await UniTask.Yield(cancellationToken: token);

                    elapsedDragDuration -= Time.unscaledDeltaTime;

                    if (elapsedDragDuration <= 0)
                    {
                        if(Vector2.Distance(Input.mousePosition, lastMousePosition) < dragThreshold * 5)
                        {
                            break;
                        }

                        elapsedDragDuration = dragEndDuration;
                        lastMousePosition = Input.mousePosition;
                    }
                }

                Vector2 screenDiagonal = new Vector2(Screen.width, Screen.height);
                EventManager.InvokeThrowBallEvent(OnInputEnded(Input.mousePosition, screenDiagonal.magnitude));

                await UniTask.WaitForSeconds(inputRefreshDuration / 2, cancellationToken: token);

                EventManager.InvokeSpawnBallEvent();

                await UniTask.WaitForSeconds(inputRefreshDuration / 2, cancellationToken: token);
            }
        }
    }
}
