using System.Collections;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private Transform _teleportTarget;
    [SerializeField] private AudioClip _teleportOutClip, _teleportInClip;
    [SerializeField] private GameObject _teleportParticlePrefab;
    [SerializeField] private float _teleportDelay = 0.5f;

    private void Awake()
    {
        _teleportTarget = transform.GetChild(0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IPlayerController controller)) return;

        Instantiate(_teleportParticlePrefab, collision.transform.position, Quaternion.identity);

        controller.RepositionImmediately(_teleportTarget.position, true);
        controller.TogglePlayer(false);

        AudioSource.PlayClipAtPoint(_teleportInClip, _teleportTarget.position);

        StartCoroutine(ActivatePlayer(controller));
    }

    private IEnumerator ActivatePlayer(IPlayerController controller)
    {
        yield return new WaitForSeconds(_teleportDelay);
        controller.TogglePlayer(true);
        Instantiate(_teleportParticlePrefab, _teleportTarget.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(_teleportOutClip, _teleportTarget.position);
    }

    private void OnDrawGizmosSelected()
    {
        _teleportTarget = transform.GetChild(0);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, _teleportTarget.position);
        Gizmos.DrawSphere(_teleportTarget.position, 0.2f);
    }
}
