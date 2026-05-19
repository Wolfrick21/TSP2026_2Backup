using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed;
    public CharacterController character;
    void Start()
    {
        character = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("HorizontaL");
        float z = Input.GetAxis("Vertical");

        Vector3 forward  = transform.right * x +  transform.forward * z;
        character.SimpleMove(forward * speed);
    }
}
