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

        // Use this for initialization
        private void Start()
        {
            _playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
            if (!_playerAnimator.GetBool("MachineSide"))
            {
                m_LastTargetPosition = target.position;
                m_OffsetZ = (transform.position - target.position).z;
                transform.parent = null;
            }
        }


        // Update is called once per frame
        private void Update()
        {
            _playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
            if (!_playerAnimator.GetBool("MachineSide"))
                CameraFollowPlayer();
        }

        private void CameraFollowPlayer() {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;


            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor * new Vector3(1, target.position.y + 1.2f, 0) * Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, new Vector3(0,target.position.y + 1.2f,0), Time.deltaTime * lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}
