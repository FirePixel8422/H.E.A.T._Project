using System.Collections;
using UnityEngine;


public class DeathBehavior : MonoBehaviour
{
    public Animator deathAnimator;

    public float cardSelectionTime;

    public GameObject upgradeCards;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartUpgradeMenus();
        }
    }

    public void StartUpgradeMenus()
    {
        StartCoroutine(FadeInUpgrades());
    }
    private IEnumerator FadeInUpgrades()
    {
        deathAnimator.SetInteger("Death", 1); //fadein/out... round win/lose... fadein/out
        
        yield return new WaitForSeconds(5f);
        upgradeCards.SetActive(true);
    }

    public void EndUpgradeMenus()
    {
        StartCoroutine(FadeOutUpgrades());
    }
    private IEnumerator FadeOutUpgrades()
    {
        deathAnimator.SetInteger("Death", 2);

        yield return new WaitForSeconds(1);
        upgradeCards.SetActive(false);
    }
}
