using UnityEngine;

public class SoundFeedback : MonoBehaviour
{
    public AudioSource src;
    public AudioClip alge, halophyte, qualle, grille, supermarkt, lose;

    public void PlaceCompartmentAlge()
    {
        src.clip = alge;
        src.Play();
    }
    public void PlaceCompartmentHalophyte()
    {
        src.clip = halophyte;
        src.Play();
    }
    public void PlaceCompartmentQualle()
    {
        src.clip = qualle;
        src.Play();
    }
    public void PlaceCompartmentGrille()
    {
        src.clip = grille;
        src.Play();
    }
    public void PlaceCompartmentSupermarkt()
    {
        src.clip = supermarkt;
        src.Play();
    }
    public void Lose()
    {
        src.clip = lose;
        src.Play();
    }
}
