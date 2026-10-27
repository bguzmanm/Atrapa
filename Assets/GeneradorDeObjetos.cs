using UnityEngine;

public class GeneradorDeObjetos : MonoBehaviour
{
    public GameObject monedaPrefab;
    public GameObject obstaculoPrefab;

    public Transform jugador;
    public Recolector recolector;

    public float intervalo = 2f;
    private float acumulado = 0f;

    void Update()
    {
        if (recolector != null && recolector.gameOver) return;

        acumulado += Time.deltaTime;

        if (acumulado >= intervalo)
        {
            if (monedaPrefab != null)
            {
                GameObject copiaMoneda = Instantiate(monedaPrefab,
                    NacerLejos(0.25f), Quaternion.identity);
                Destroy(copiaMoneda, 10f);
            }

            if (obstaculoPrefab != null)
            {
                GameObject copiaObstaculo = Instantiate(obstaculoPrefab,
                    NacerLejos(0.5f), Quaternion.identity);
                Destroy(copiaObstaculo, 10f);
            }

            acumulado = 0f;
            intervalo = Mathf.Max(1.2f, intervalo * 0.95f);
        }
    }

    Vector3 NacerLejos(float altura)
    {
        Vector3 pos;
        if (jugador == null)
        {
            pos = new Vector3(Random.Range(-6f, 6f), altura, Random.Range(-5f, 5f));
            return pos;
        }

        do
        {
            pos = new Vector3(Random.Range(-6f, 6f), altura, Random.Range(-5f, 5f));
        }
        while (Vector3.Distance(pos, jugador.position) < 3f);

        return pos;
    }
}
