using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera m_cam;
        [SerializeField] private float m_horizontalBounds;
        [SerializeField] private float m_verticalBounds;
        [SerializeField] private float m_zoomLimits;
        [SerializeField] private float m_panSpeed;

        private Vector3 m_camStartPos;
        private float m_camStartZoom;

        private void Awake() {
            if (m_cam == null) {
                m_cam = this.GetComponent<Camera>();
            }
            m_camStartPos = m_cam.transform.position;
            m_camStartZoom = m_cam.orthographicSize;
        }

        private void Update() {
            Vector3 moveVector;

            if (Input.GetKey(KeyCode.W)) {
                // forward / up
                moveVector = new Vector3(1, 0, 0) * m_panSpeed * Time.deltaTime;
               
                if ((m_cam.transform.position + moveVector).x > m_camStartPos.x + m_verticalBounds) {
                    m_cam.transform.position = new Vector3(
                        m_camStartPos.x + m_verticalBounds,
                        m_cam.transform.position.y,
                        m_cam.transform.position.z
                        );
                }
                else {
                    m_cam.transform.position += moveVector;
                }
            }
            if (Input.GetKey(KeyCode.S)) {
                // backward / down
                moveVector = new Vector3(-1, 0, 0) * m_panSpeed * Time.deltaTime;

                if ((m_cam.transform.position + moveVector).x < m_camStartPos.x - m_verticalBounds) {
                    m_cam.transform.position = new Vector3(
                        m_camStartPos.x - m_verticalBounds,
                        m_cam.transform.position.y,
                        m_cam.transform.position.z
                        );
                }
                else {
                    m_cam.transform.position += moveVector;
                }
            }
            if (Input.GetKey(KeyCode.A)) {
                // left
                moveVector = new Vector3(0, 0, 1) * m_panSpeed * Time.deltaTime;

                if ((m_cam.transform.position + moveVector).z > m_camStartPos.z + m_horizontalBounds) {
                    m_cam.transform.position = new Vector3(
                        m_cam.transform.position.x,
                        m_cam.transform.position.y,
                        m_camStartPos.z + m_horizontalBounds
                        );
                }
                else {
                    m_cam.transform.position += moveVector;
                }
            }
            if (Input.GetKey(KeyCode.D)) {
                // right
                moveVector = new Vector3(0, 0, -1) * m_panSpeed * Time.deltaTime;

                if ((m_cam.transform.position + moveVector).z < m_camStartPos.z - m_horizontalBounds) {
                    m_cam.transform.position = new Vector3(
                        m_cam.transform.position.x,
                        m_cam.transform.position.y,
                        m_camStartPos.z - m_horizontalBounds
                        );
                }
                else {
                    m_cam.transform.position += moveVector;
                }
            }
            if (Input.GetKey(KeyCode.J)) {
                // zoom in
                m_cam.orthographicSize = Mathf.Max(
                    m_cam.orthographicSize - 1 * m_panSpeed * Time.deltaTime,
                    m_camStartZoom - m_zoomLimits
                    );
            }
            if (Input.GetKey(KeyCode.K)) {
                // zoom out
                m_cam.orthographicSize = Mathf.Min(
                    m_cam.orthographicSize + 1 * m_panSpeed * Time.deltaTime,
                    m_camStartZoom + m_zoomLimits
                    );
            }


        }
    }
}