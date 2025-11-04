using System.Collections;
using UnityEngine;


public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3[] localPositions;

    [SerializeField] private MinMaxFloat moveDelay;
    [SerializeField] private MinMaxFloat moveTime;



    private void Start()
    {
        StartCoroutine(MoveLoop());
    }

    private IEnumerator MoveLoop()
    {
        float elapsed = 0;
        float cMoveTime = EzRandom.Range(moveTime);

        float cDelay = EzRandom.Range(moveDelay);
        yield return new WaitForSeconds(cDelay);

        int previousPosId = 0;
        int posId = 0;

        while (true)
        {
            yield return null;
            elapsed += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(localPositions[previousPosId], localPositions[posId], elapsed / cMoveTime);

            if (elapsed >= cMoveTime)
            {
                elapsed = 0;
                previousPosId = posId;
                posId += 1;

                if (posId == localPositions.Length)
                {
                    posId = 0;
                }

                cMoveTime = EzRandom.Range(moveTime);
                cDelay = EzRandom.Range(moveDelay);
                yield return new WaitForSeconds(cDelay);
            }
        }
    }
}
