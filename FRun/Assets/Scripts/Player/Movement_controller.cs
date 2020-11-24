using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Animator))] //юніті буде автоматично додавати на даний об'єк (ми закинули цей скрипт на плеєра) компоненту типу Rigidbody2D (щоб не страшно було якщо хтось в юніті видалить цю компоненту)
public class Movement_controller : MonoBehaviour
{

    private Animator _playerAnimator;
    //SerializeField атрибут з юніті, який відериває компоненту для редагування з інспектора
    Rigidbody2D _playerRigidbody2D;//компонент відповідаючий за фізику об'єкта
    private Player _playerController;

    [Header("Horizontal movement")]//заголовок в інспекторі
    [SerializeField] private float _speed;//швидкість руху перса
    private bool _faceRight = true;

    [Header("Jumping")]//заголовок в інспекторі
    [SerializeField] private float _jumpForce;//сила стрибку (те на скільки юнітах(?) перенести персонажа вгору 
    [SerializeField] private Transform _groundCheck;//точка простору в якій знаходиться визначувач поверхні під ногами або над головою
    [SerializeField] private float _radius;//радіус _groundCheck або _cellCheck
    [SerializeField] private bool _airControll;//в залежності від стану чекбоксу в інспекторі дозволяємо або забороняємо зміну руху у повітрі
    private bool _grounded;//змінна яка відповідає за стан приземлення

    [Header("Crawling")]//заголовок в інспекторі
    [SerializeField] private Transform _cellCheck;//перевірка голови)
    [SerializeField] private LayerMask _whatIsGround;//поле яке містить у собі певний шар (layer) в юніті 
    [SerializeField] private Collider2D _headCollider;//колайдер для голови
    private bool _canStand;//чи можемо встати (чи не знаходимося присівши під стелею)

    //5 типів даних з якими любить працювати юніті: float, bool, ...

    [Header("Shooting")]
    [SerializeField] private GameObject _fireBall;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireBallSpeed;
    [SerializeField] private int _shootingCost;
    private bool _isShooting;

    [Header("Strike")]
    [SerializeField] private Transform _strikePoint;
    [SerializeField] private int _damage;
    [SerializeField] private float _strikeRange;
    [SerializeField] private LayerMask _enemies;
    [SerializeField] private int _strikeCost;
    private bool _isStriking;


    //Awake відпрацює раніше Start (відпрацює післязавантаження ресурсів і до того як стартує сцена)
    private void Awake()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
    }
    // Start is called before the first frame update (запускається 1 раз, відпрацьовує після того як завантажились ресурси і до першого кадру системи)
    void Start()
    {
        _playerController = GetComponent<Player>();
    }
    
    //FixedUpdate схожий на Update, але відпрацьовує через вказаний (в юніті) проміжок часу (0.02 сек)
    private void FixedUpdate() //сюди записуємо всю логіку пов'язану з фізикою об'єкта (правило юніті)
    {
        


    }
    //спосіб візуалізації в юніті
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundCheck.position, _radius);//малює порожню сферу з центром _groundCheck.position та вказаним радіусом
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_cellCheck.position, _radius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(_strikePoint.position, _strikeRange);
    }

    void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    public void Move(float move, bool jump, bool crawling)
    {
        #region Movement
        
        if (move != 0 && (_grounded || _airControll))
        {
            _playerRigidbody2D.velocity = new Vector2(_speed * move, _playerRigidbody2D.velocity.y);
        }

        if (move > 0 && !_faceRight)
        {
            Flip();
        }
        else if (move < 0 && _faceRight)
        {
            Flip();
        }
        #endregion
        #region Jumping
        _grounded = Physics2D.OverlapCircle(_groundCheck.position, _radius, _whatIsGround);//OverlapCircle повертає true якщо колайдер знаходиться на шарі який визначає _whatIsGround і false якщо не знаходиться

        //_playerRigidbody2D.velocity = new Vector2(_speed * _move, _playerRigidbody2D.velocity.y);
        if (jump && _grounded)
        {
            _playerRigidbody2D.AddForce(Vector2.up * _jumpForce);
            //jump = false; - цей джамп не повертається в PC_inputController щоб змінити _jump на false тому тут він не потрібен а от там потрібен
        }
        #endregion
        #region Crawling
        _canStand = !Physics2D.OverlapCircle(_cellCheck.position, _radius, _whatIsGround);//якщо колайдера в наявності над головою нема то можемо стояти

        if (crawling)
        {
            _headCollider.enabled = false;
        }
        else if (!crawling && _canStand)
        {
            _headCollider.enabled = true;
        }
        #endregion

        #region Animation
        _playerAnimator.SetFloat("Speed", Mathf.Abs(move));
        _playerAnimator.SetBool("Jump", !_grounded);
        _playerAnimator.SetBool("Crouch", !_headCollider.enabled);
        #endregion
    }

    public void StartShooting()
    {
        if (_isShooting || !_playerController.ChangeMP(-_shootingCost))
            return;
        _isShooting = true;
        _playerAnimator.SetBool("Shoot", true);
    }

    private void ShootFire()
    {
        GameObject fireBall = Instantiate(_fireBall, _firePoint.position, Quaternion.identity);
        fireBall.GetComponent<Rigidbody2D>().velocity = transform.right * _fireBallSpeed;
        fireBall.GetComponent<SpriteRenderer>().flipX = !_faceRight;
        Destroy(fireBall, 5f);
    }

    private void EndShooting()
    {
        _isShooting = false;
        _playerAnimator.SetBool("Shoot", false);
    }

    public void StartStrike()
    {
        if (_isStriking)
            return;

        if (!_playerController.ChangeMP(-_strikeCost))
            return;

        _playerAnimator.SetBool("Strike", true);
        _isStriking = true;
    }

    public void Strike()
    { 
        Collider2D[] enemies = Physics2D.OverlapCircleAll(_strikePoint.position, _strikeRange, _enemies);
        for(int i = 0; i < enemies.Length; i++)
        {
            EnemiesController enemy = enemies[i].GetComponent<EnemiesController>();
            enemy.TakeDamage(_damage);
        }
    }

    private void EndStrike()
    {
        _playerAnimator.SetBool("Strike", false);
        _isStriking = false;
    }
}








/* velocity - швидкість, бистрота
 * force - сила
 * crawling - повзання
 * enabled - увімкнено
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */