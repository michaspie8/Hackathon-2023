using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMotor))]
public class EnemyController : MonoBehaviour
{
    public Enemy enemystats;
    public LayerMask movmentMask;
    public GameObject player => PlayerController.instance.gameObject;
    EnemyMotor motor;

    float seeRange = 0f;
    bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<EnemyMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) >= enemystats.range) motor.MoveToPoint(player.transform.position);
        else motor.MoveToPoint(transform.position);
        if (Vector3.Distance(player.transform.position, transform.position) <= (enemystats.range + 1) && !isAttacking) Attack();
    }

    bool IsPlayerReachable()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= seeRange && Vector3.Distance(player.transform.position, transform.position) >= enemystats.range) return true;
        return false;
    }
    IEnumerator fff()
    {
        isAttacking = true;
        yield return new WaitForSeconds(1);
        PlayerController _playerController = player.GetComponent<PlayerController>();
        _playerController.DecreaseHealth(enemystats.damage);
        Debug.Log(_playerController.health);
        isAttacking = false;

    }
    void Attack()
    {
        StartCoroutine(fff());

    }
}
