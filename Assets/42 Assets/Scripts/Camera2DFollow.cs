using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        private Animator _playerAnimator;

        private Camera _camera;

        //ZOOM
        private float zoomSpeed = 1;
        private float targetOrtho;
        private float smoothSpeed = 2.0f;
        private float minOrtho = 1.0f;
        private float maxOrtho = 20.0f;

        // Use this for initialization
        private void Start()
        {
            _playerAnimator = target.GetComponent<Animator>();
            _camera = GetComponent<Camera>();

            if (!_playerAnimator.GetBool("MachineSide"))
            {
                m_LastTargetPosition = target.position;
                m_OffsetZ = (transform.position - target.position).z;
                transform.parent = null;
            }
            //ZOOM
            targetOrtho = Camera.main.orthographicSize;
        }

        // Update is called once per frame
        private void Update()
        {
            _playerAnimator = target.GetComponent<Animator>();

            if (_playerAnimator.GetBool("MachineSide")) { 
                /*
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0.0f)
                {
                    targetOrtho -= scroll * zoomSpeed;
                    targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
                    Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
                }*/

                Vector3 viewPosition = _camera.WorldToViewportPoint(target.position);
                float xMoveDelta = (target.position - m_LastTargetPosition).x;

                if (viewPosition.x > 0.5f)
                {
                    m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);

                    Vector3 nastyHack = new Vector3(target.position.x, transform.position.y, transform.position.z);
                    Vector3 aheadTargetPos = nastyHack + m_LookAheadPos + Vector3.forward * m_OffsetZ;
                    Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, 2.0f);

                    transform.position = newPos;
                    m_LastTargetPosition = transform.position;
                }
                else {
                    if (viewPosition.x < 0.3f && Mathf.Sign(xMoveDelta) == -1)
                    {
                        m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);

                        Vector3 nastyHack = new Vector3(target.position.x, transform.position.y, transform.position.z);
                        Vector3 aheadTargetPos = nastyHack + m_LookAheadPos + Vector3.forward * m_OffsetZ;
                        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, 3.0f);

                        transform.position = newPos;
                        m_LastTargetPosition = transform.position;
                    }
                } 

            } else CameraFollowPlayer();
        }

        private void CameraFollowPlayer() {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
            }

            Vector3 nastyHack = new Vector3(target.position.x + 2.0f, target.position.y + 0.6f, target.position.z);
            Vector3 aheadTargetPos = nastyHack + m_LookAheadPos + Vector3.forward * m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}
