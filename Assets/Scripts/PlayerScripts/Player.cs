using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Player : MonoBehaviour 
 {
   [SerializeField]  public float speed =2.5F;
   public float Speed
   {
     get{return speed;}
     set
     {
      if (value >4)
      speed =value;
     }
   }

   [SerializeField]private float force;
   [SerializeField]private Rigidbody2D rigidboby;
   [SerializeField] private float minimalHeight;
   public float minimalheight
   {
    get{return minimalHeight;}

   }
   [SerializeField]private bool isCheatMode;
   [SerializeField]private GroundDetection groundDetection;
   private Vector3 direction;
   [SerializeField] private Animator animator;
   [SerializeField] private SpriteRenderer spriteRenderer;
   private bool isJumping;
   [SerializeField] private Arrow arrow;
   [SerializeField] private Transform arrowSpawnPoint;
   [SerializeField] private float shootForce = 5;
   private bool canShoot;
   [SerializeField] private float cooldown;
   [SerializeField] private float damageForce;
   [SerializeField] private Arrow currentarrow;
   private float bonusForce;
   private float bonusHealth;
   private float extraDamage;
   private List<Arrow> arrowPooll;
   [SerializeField] private int ArrowsCount =3;
   [SerializeField] private Health health;
   public Health Health { get{return health; } }
   [SerializeField] private Item item;
   [SerializeField] private BuffReciever buffReciever;
   [SerializeField] private Camera playerCamera;
   private bool isBlockMovement;
    private UICharacterController controller;
    public GameObject fire;
    public GameObject reload;
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioClip ArrowSound;
    [SerializeField] private AudioClip PlayerDamageWoman;
    private void Awake()
    {
        Instance = this;
    }

   private void Start()
   {
       
        reload.SetActive(false);
     arrowPooll =new List<Arrow>();
     for (int i=0; i<ArrowsCount; i++)
     {
       var arrowTemp =Instantiate (arrow,arrowSpawnPoint);
       arrowPooll.Add(arrowTemp);
       arrowTemp.gameObject.SetActive(false);
     }
     health.OnTakeHit +=TakeHit;
     buffReciever.OnBuffsChanged += NewBuffs;
   }

   #region Singleton
    public static Player Instance { get; set; }
    #endregion
   public void InitUIController(UICharacterController uiController)
   {
    controller=uiController;
    controller.Fire.onClick.AddListener(CheckShoot);
   }

   private void NewBuffs()
   {
    var forceBuff = buffReciever.Buffs.Find(t=> t.type == BuffType.Force);
    var damageBuff =buffReciever.Buffs.Find(t=> t.type == BuffType.Damage);
    var armorBuff = buffReciever.Buffs.Find(t=> t.type == BuffType.Armor);
    bonusForce =  forceBuff == null ? 0 : forceBuff.additiveBonus;
    bonusHealth = armorBuff == null ? 0 : armorBuff.additiveBonus;
    health.SetHealth((int)bonusHealth);
    extraDamage = damageBuff == null ? 0 : damageBuff.additiveBonus; 
   }
   private void TakeHit(int damage, GameObject attacker)
   {
     animator.SetBool("Damaged",true);
        animator.SetTrigger("TakeHit");
        soundSource.PlayOneShot(PlayerDamageWoman);
        isBlockMovement = true;
     rigidboby.AddForce(transform.position.x < attacker.transform.position.x 
     ? new Vector2(-damageForce,0): new Vector2(damageForce,0),ForceMode2D.Impulse);
   }
   public void UnblockMovement()
   {
     isBlockMovement =false;
     animator.SetBool("Damaged",false);
   }
    private void FixedUpdate()
   {
    Move();
 #if UNITY_EDITOR 
 if(Input.GetKey(KeyCode.Space))
  OnJumpButtonDown();
#endif
   animator.SetFloat("Speed",Mathf.Abs(direction.x));
   CheckFall();
   }
   public void OnJumpButtonDown()
    {
        
        
        if (groundDetection.isGrounded)
        {
            rigidboby.AddForce(Vector2.up * (force + bonusForce), ForceMode2D.Impulse);
            animator.SetTrigger("StartJump");
            isJumping = true;
        }

    }
 private void Update()
 {
  
 }
 public void Move()
 {
   animator.SetBool("isGrounded",groundDetection.isGrounded);
    if(!isJumping && !groundDetection.isGrounded)
    {
      animator.SetTrigger("StartFall");
    }

    isJumping = isJumping && !groundDetection.isGrounded;
        direction = Vector3.zero;
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A))
            direction = Vector3.left;
        if (Input.GetKey(KeyCode.D))
            direction = Vector3.right;
#endif 
        if (controller.Left.IsPressed)
            direction = Vector3.left;
        if (controller.Right.IsPressed)
            direction = Vector3.right;
        direction *= speed;
        direction.y = rigidboby.velocity.y;
        if (!isBlockMovement)
        rigidboby.velocity = direction;

        if (direction.x > 0)
            spriteRenderer.flipX = false;
        if (direction.x < 0)
            spriteRenderer.flipX = true;
    }

   private void CheckShoot()
    {
        if (!canShoot)
        {
            animator.SetTrigger("StartShoot");
        }
    }

  public void InitArrow()
    {
        currentarrow = GetArrowFromPool();
        currentarrow.SetImpulse(Vector2.right, 0, 0, this);
    }
private void Shoot()
    {
        currentarrow.SetImpulse
                (Vector2.right, spriteRenderer.flipX ?
                -force * shootForce : force * shootForce, (int)extraDamage, this);
        soundSource.PlayOneShot(ArrowSound);
        StartCoroutine(Reload());
    }  

  private IEnumerator Reload()
{
  reload.SetActive(true);
  canShoot = true;
  yield return new WaitForSeconds(cooldown);
  canShoot = false;
  reload.SetActive(false);
}
  private Arrow GetArrowFromPool()
  {
    if(arrowPooll.Count > 0)
    {
      var arrowTemp = arrowPooll[0];
      arrowPooll.Remove(arrowTemp);
      arrowTemp.gameObject.SetActive(true);
      arrowTemp.transform.parent=null;
      arrowTemp.transform.position =arrowSpawnPoint.transform.position;
      return arrowTemp;
    }
    return Instantiate
      (arrow,arrowSpawnPoint.position,Quaternion.identity);
  }
  public void ReturnArrowToPool(Arrow arrowTemp)
  {
  if (arrowPooll.Contains(arrowTemp))
  arrowPooll.Add(arrowTemp);
  arrowTemp.transform.parent =arrowSpawnPoint;
  arrowTemp.transform.position =arrowSpawnPoint.transform.position;
  arrowTemp.gameObject.SetActive(false);

  }

   void CheckFall()
   {
        if (transform.position.y < minimalHeight && isCheatMode)
        {
            rigidboby.velocity = new Vector2(0, 0);
            transform.position = new Vector2(0, 0);
        }
        else if (transform.position.y < minimalHeight)
        Destroy(gameObject);
        

    }

    private void OnDestroy()
    {
       
       playerCamera.transform.parent = null;
       playerCamera.enabled = true;
    }
}
 

 
 
   

	
 
