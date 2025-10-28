using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DeathBehavior : MonoBehaviour
{
    public Animator deathAnimator;

    public float cardSelectionTime;

    public GameObject upgradeCards;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            OnDeathBehavior();
        }
    }
    public void OnDeathBehavior()
    {
        StartCoroutine(CardSelectionWait(cardSelectionTime));
    }
    IEnumerator CardSelectionWait(float time)
    {
        deathAnimator.SetInteger("Death", 1);
        
        yield return new WaitForSeconds(5f);
        upgradeCards.SetActive(true);

        yield return new WaitForSeconds(time - 1f);
        deathAnimator.SetInteger("Death", 2);

        yield return new WaitForSeconds(1);
        upgradeCards.SetActive(false);
    }

}
