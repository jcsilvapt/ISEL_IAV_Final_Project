using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [Header("Bullet Settings")]
    public float fireRate = 2f;



    [Header("Developer Settings")]
    public bool enableDebugs;
    public bool enableMic;
    public string deviceName;
    public float elapsedTime = 0f;
    public bool canShoot;

    private AudioSource aSource;

    private void Start() {

        aSource = GetComponent<AudioSource>();

        if (enableMic) {
            if (Microphone.devices.Length > 0) {
                deviceName = Microphone.devices[0];
                aSource.clip = Microphone.Start(deviceName, true, 10, AudioSettings.outputSampleRate);
                while (Microphone.GetPosition(deviceName) < 48 * 30) ;
            } else {
                enableMic = false;
            }
        }

        if (enableMic) {
            aSource.loop = true;
            aSource.Play();
            StartCoroutine(MicInteraction());
        }

        elapsedTime = fireRate;
        canShoot = true;

    }

    private void Update() {
        if (!canShoot) {
            if (elapsedTime >= fireRate) {
                canShoot = true;
                elapsedTime = 0f;
            } else {
                elapsedTime += Time.deltaTime;
            }
        }
    }

    IEnumerator MicInteraction() {
        while (true) {
            yield return null;
            float energy = AudioAnalysis.MeanEnergy(aSource);

            if (AudioAnalysis.ConvertToDB(energy) > 40) {
                float peakFrequency = AudioAnalysis.ComputeSpectrumPeak(aSource, true);
                float concentration = AudioAnalysis.ConcentrationAroundPeak(peakFrequency);

                if (enableDebugs) Debug.Log("Concentration: " + concentration);

                if (concentration > 0.1f) {
                    if (enableDebugs) Debug.Log("Shooting");
                    SpawnBullet();
                }
            }
        }
    }

    private void SpawnBullet() {

        if(canShoot) {
            canShoot = false;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        }

    }

}
