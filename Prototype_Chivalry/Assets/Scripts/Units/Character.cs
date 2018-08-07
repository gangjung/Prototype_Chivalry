using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour{

    public CharacterStat Stat { get { return _stat; } }
    public CharacterMovement Movement { get { return _movement; } }
    public Transform Position { set { _position = value; } }
    public int PositionNumber;  // 현재 포지션 넘버.

    public GameObject asdf;

    private CharacterStat _stat;
    private CharacterMovement _movement;    // Movement
    private Transform _position;
    private List<int> _buffList;    // Buff List

    // Skill

    // 생성자에서는 Mono문법 사용 불가능. ex) transform.position 등
    // 그래서 Awake에 해준다.
    private void Awake()
    {
        asdf = Resources.Load("Weapons/Cube") as GameObject;
        _position = transform.parent;

        _stat = new CharacterStat("홍길동", 10, 10, 10, 10, 10, 5);
        _movement = new CharacterMovement(transform, _position);
    }

    private void Update()
    {

        Debug.Log(Movement.Return);
        if (Movement.Return == true)
        {
            Movement.Returning();
        }
    }

    
    public Character()
    {
        
    }
    
    public void Move()
    {
        _movement.Move();
    }

    public void Attack()
    {

    }

    public void UseSkill()
    {
        if (Movement.Skill == true)
            return;

        Movement.UseSkill();

        transform.parent = null;
        // 스킬 사용이 끝나면 Movement.Return = true;
        StartCoroutine(SkillSkill());
    }

    public void Attacked()
    {

    }

    private IEnumerator SkillSkill()
    {
        yield return new WaitForSeconds(0.3f);

        Instantiate(asdf, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.3f);
        Instantiate(asdf, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.3f);
        Instantiate(asdf, transform.position, transform.rotation);

        Movement.Return = true;
    }

    // 충돌처리하는 과정에서, Enter이기 때문에, Return == true 되고나서 계속 Player와 마찰 중이라면 해당 코드가 실행되지 않음.
    // 스킬쓰고 복귀 속도가 더 빠른 상황에서, 계속 마찰 중이라면, 절대로 Enter가 실행되지 않는다.
    // 그것을 방지하기 위해, Player와 Character간의 거리를 늘려주거나, object크기를 작게 만들면 된다.
    // 아니면 Enter가 아니라 Stay를 해도 상관 없다.
    // 이걸 방지하기위해, 아예 함수를 사용하지 않고, 거리 측정을 하도록 했음.
    private void OnTriggerEnter(Collider other)
    {
        if (Movement.Return == true)
        {
            if (other.tag == "Player")
            {
                
            }
        }
    }
}
