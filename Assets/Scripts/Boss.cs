using UnityEngine;
using System.Collections;

// Interpretaremos la inteligencia artificial como una maquina de estados.
public class Boss : FSM {
	//Se nombran los estados de la maquina
	public enum fsmState{
		None,
		persecusion,
		rayo,
		espada,
		brinco,
		stands,
		retroceder,
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	// Variable que contiene las animaciones.
	Animator animador;
	// Se declara la variable del mismo estado del enum fsmState.
	public fsmState estado;
	// Se declara el prefabs o municion que se utilizara para el antagonista.
	public GameObject beam;
	// se declara unos de los atibutos mas importantes, la salud del antagonista
	private float curspeed;
	public float Distancia = 0f;
	public bool Pego = false;
	public bool ataque = false;
	public bool Disparo = false;
	public float tiempo= 0.5f;
	/* una vez declarado los atributos del antagonista se comienza a crear los estados de la maquina finita.
    sobreescribiremos el metodo que se encuentra en FSM(clase padre).
   */
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake (){
		// Se hace instanciacion de las animaciones.
		animador = GetComponent <Animator>();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Initialize(){
		// Se asigna el primer estado, el cual es el de persecusion.
		estado = fsmState.persecusion;
		// Se le asigna la velocidad
		curspeed = 6.0f;
		// Es el tiempo trasncurrido, se encuentra en fsm ya que boss se hereda de esta clase.
		elapsedTime = 0.0f;
		shootRate = 4f;
		// Esta instancia busca al objeto player
		GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
		// Accedemos a la propiedad transform del "Player".
		playerTransform = objPlayer.transform;
		// Accedemos a la propiedad transform del primer objeto hijo para se usado como posicion del spawn.
		bulletSpawnPoint = gameObject.transform.GetChild (0);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void FSMUpdate(){   
		// Los diferentes estados que puede tomar la inteligencia artificial.
		switch (estado){
		case fsmState.persecusion: UpdatePersecusion(); break;
		case fsmState.retroceder: UpdateRetroceder(); break;
		case fsmState.rayo: UpdateRayo(); break;
		case fsmState.espada: UpdateEspada(); break;
		case fsmState.brinco: UpdateBrinco(); break;
		case fsmState.stands: UpdateStand(); break;
		}	
		// El tiempo transcurrido
		elapsedTime += Time.deltaTime;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdatePersecusion(){
		destPos = playerTransform.position;
		//mantiene la persecusion
		curspeed = 6.0f;
		if (Vector2.Distance(transform.position, destPos) <= 10.0f){
			print("Me encuentro en estado stand y cambiare a persecusion");
			//! Hay que modificar aca para que puede perseguir en ambas direcciones.
			transform.Translate(Vector3.right * Time.deltaTime * curspeed);
		}
		// Decide si esta cerca cambia al estado de ataque
		if (Vector2.Distance(transform.position, playerTransform.position) <= 10.0f){	
			print("cambio a modo de ataque");
			ataque = true;
			animador.SetBool ("Cerca",ataque);
			estado= fsmState.espada;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateEspada(){
		transform.Translate(Vector3.right * Time.deltaTime * curspeed);
		if (Vector2.Distance(transform.position, playerTransform.position) <= 2.0f)
		{
			print("cambio a modo de retroceso");
			Pego= true;
			animador.SetBool ("Pego",Pego);
			estado= fsmState.retroceder;	
		}
		if (Vector2.Distance(transform.position, playerTransform.position) == 16.0f){
			curspeed=0;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateBrinco(){
		print ("Estoy brincando");
	}
	protected void UpdateRayo(){
		print ("kameha");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateRetroceder(){
		transform.Translate(Vector3.left * Time.deltaTime * curspeed);
		if (Vector2.Distance(transform.position, playerTransform.position)>= 16.0f){
			estado = fsmState.stands;
			Disparo=true;
			animador.SetBool("Disparo",Disparo);
			ShootBullet();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateStand(){
		curspeed = 0;
		transform.Translate(Vector3.left * Time.deltaTime * curspeed);
		Pego = false ;
		ataque = false;
		animador.SetBool ("Cerca",ataque);
		animador.SetBool ("Pego",Pego);
		if(Vector2.Distance(transform.position, playerTransform.position) <=16.0f){
			Disparo = false;
			animador.SetBool ("Disparo", Disparo);
			estado = fsmState.persecusion;
		}
		
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ShootBullet (){
		if (elapsedTime >= shootRate){
			Instantiate (beam, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
			elapsedTime = 0.0f;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	/* detecta las colision con los distintos elementos, en este caso detectara si el Player le arroja algun objeto que inflija 
      dano
	 */
	void OnTriggerEnter2D(Collider2D collider){
		/*normalmente en los videojuegos el personaje principal cuenta con diferentes tipos de armas
 		segun la naturaleza del dano pueden quitar diferentes tipos de cantidad a la salud del antagonista
        pero llevar un control del objeto que colisiona con el antagonista disponemos a ponerle un TAG al
        prefabs o colisionador que estamos utilizando, en este caso utilizaremos el Hadouken(tecnica de Ryu)
		*/
		if(collider.tag == "Hadouken"){
			NotificationCenter.DefaultCenter ().PostNotification (this, "impactado");
		}
	}
}


