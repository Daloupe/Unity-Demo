using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Tree))]
public class TreeScript : MonoBehaviour, IPointerClickHandler
{
    //public Vector2 maxHeight = new Vector2(44, 50);
    //Vector2 originalHeight;

    //public GameObject TreePrefab;
    //public Tree tree;
    //TreeEditor.TreeData treeData;
    //TreeEditor.TreeGroupBranch treeBranch;

    public GameObject portalGroup;
    void Awake()
    {
        //portalGroup = GameObject.Find("Portal Group");
        //tree = TreePrefab.GetComponent<Tree>();
        ////foreach (var c in tree.GetComponentsInChildren())
        //treeData = (tree.data as TreeEditor.TreeData);
        //treeBranch = treeData.branchGroups[2];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        portalGroup.SetActive(true);
        //Debug.Log("Clicked " + treeBranch.height);
        //StopAllCoroutines();
        //StartCoroutine(GrowBranch());
    }

    //IEnumerator GrowBranch()
    //{
    //    Debug.Log("Growing");
    //    originalHeight = treeBranch.height;
    //    float elapsedTime = 0;
    //    float time = 2;

    //    Debug.Log(treeBranch.height);
    //    while (elapsedTime < time)
    //    {
    //        //treeBranch.Lock();
    //        treeBranch.height = Vector2.Lerp(originalHeight, maxHeight, (elapsedTime / time));
    //        //treeBranch.Unlock();
    //        elapsedTime += Time.deltaTime;
    //        tree.data.SetDirty();
    //        //treeData.Initialize();
    //        //treeData.CheckExternalChanges();
    //        yield return new WaitForEndOfFrame();
    //    }
    //    Debug.Log(treeBranch.height);

    //    elapsedTime = 0;
    //    while (elapsedTime < time)
    //    {
    //        //treeBranch.Lock();
    //        treeBranch.height = Vector2.Lerp(maxHeight, originalHeight, (elapsedTime / time));
    //        //treeBranch.Unlock();
    //        elapsedTime += Time.deltaTime;
    //        //treeBranch.UpdateDistribution(true, true);
    //        tree.data.SetDirty();
    //        //treeData.CheckExternalChanges();
    //        yield return new WaitForEndOfFrame();
    //    }
    //    Debug.Log(treeBranch.height);
    //}

    //IEnumerator GrowBranch()
    //{
    //    Debug.Log("Growing");
    //    originalHeight = treeBranch.height;
    //    float timer = 2;

    //    while (treeBranch.height.y <= maxHeight.y - 1)
    //    {
    //       //timer -= Time.deltaTime;
    //        treeBranch.height = Vector2.Lerp(originalHeight, maxHeight, timer * Time.deltaTime);
    //        treeData.ValidateReferences();
    //        treeData.SetDirty();
    //       //treeBranch.UpdateParameters();
    //       //treeBranch.CheckExternalChanges();
    //       Debug.Log(treeBranch.height);
    //       yield return null;
    //    }

    //    //timer = 2;
    //    //Debug.Log(treeBranch.height);
    //    while (treeBranch.height.y >= originalHeight.y + 1)
    //    {
    //        //timer -= Time.deltaTime;
    //        treeBranch.height = Vector2.Lerp(maxHeight, originalHeight, timer * Time.deltaTime);
    //        //treeData.ValidateReferences();
    //        //treeBranch.CheckExternalChanges();
    //        treeData.SetDirty();
    //        Debug.Log(treeBranch.height);
    //        yield return null;
    //    }
    //    //Debug.Log(treeBranch.height);
    //}
}
