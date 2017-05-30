using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform player;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        public BoxCollider2D cameraBounds;
        private Vector3 _min, _max;
        public float margin;

        private Camera _camera;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        private Animator _playerAnimator;

        // Use this for initialization
        private void Start()
        {
            _playerAnimator = player.GetComponent<Animator>();

            if (!_playerAnimator.GetBool("MachineSide"))
            {
                m_LastTargetPosition = player.position;
                m_OffsetZ = (transform.position - player.position).z;
                transform.parent = null;
            }

            _camera = GetComponent<Camera>();
            _min = cameraBounds.bounds.min;
            _max = cameraBounds.bounds.max;
        }

        // Update is called once per frame
        private void Update()
        {
            _playerAnimator = player.GetComponent<Animator>();
            if (_playerAnimator.GetBool("MachineSide"))
                BadPlayerCamera();
            else GoodPlayerCamera();
        }

        private void BadPlayerCamera() {
            float xMoveDelta = (player.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
            }

            var x = transform.position.x;
            var y = transform.position.y;

            var cameraHalfWidth = _camera.orthographicSize * ((float)Screen.width / Screen.height);
            
            if (Mathf.Abs(x - player.position.x) >= cameraHalfWidth)
                x = player.position.x - (Mathf.Sign(x - player.position.x) * cameraHalfWidth);

            if (Mathf.Abs(y - player.position.y) > margin)
                y = player.position.y;// Mathf.Lerp(y, player.position.y, smoothing * Time.deltaTime);

            x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
            y = Mathf.Clamp(y, _min.y + _camera.orthographicSize, _max.y - _camera.orthographicSize);

            Vector3 nastyHack = new Vector3(x, y, transform.position.z);
            Vector3 aheadTargetPos = nastyHack + m_LookAheadPos + Vector3.forward * m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, nastyHack, ref m_CurrentVelocity, damping * Time.deltaTime);

            transform.position = newPos;

            m_LastTargetPosition = player.position;
        }

        private void GoodPlayerCamera()
        {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (player.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
            }

            var x = transform.position.x;
            var y = transform.position.y;

            var cameraHalfWidth = _camera.orthographicSize * ((float)Screen.width / Screen.height);

            x = player.position.x;
            y = player.position.y;

            x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
            y = Mathf.Clamp(y, _min.y + _camera.orthographicSize, _max.y - _camera.orthographicSize);

            Vector3 nastyHack = new Vector3(x, y, player.position.z);
            Vector3 aheadTargetPos = nastyHack + m_LookAheadPos + Vector3.forward * m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = player.position;
        }
    }
}