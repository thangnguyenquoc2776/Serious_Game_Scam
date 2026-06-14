// using System;
// using System.Collections;
// using UnityEngine;
// using SeriousGame.Content;

// namespace SeriousGame.Runtime
// {
//     public class LeadController : MonoBehaviour
//     {
//         public Transform homeAnchor;   // vị trí bàn sếp
//         public Transform talkAnchor;   // vị trí đứng nói chuyện
//         public float moveSpeed = 1.5f;
//         Animator animator;


//         private void Start()
//         {
//             animator = GetComponent<Animator>();
//         }
//         public void GoTalkThenReturn(InteractionSO dialogue, Action onDone)
//         {
//             StartCoroutine(Flow(dialogue, onDone));
//         }

//         IEnumerator Flow(InteractionSO dialogue, Action onDone)
//         {
//             // 1️⃣ Đi tới bàn player
//             yield return MoveTo(talkAnchor.position);

//             // 2️⃣ MỞ DIALOGUE
//             EpisodeController.Instance.beatRunner
//                 .RunInteraction(dialogue, () =>
//                 {
//                     // 3️⃣ NÓI XONG → QUAY VỀ
//                     StartCoroutine(ReturnHome(onDone));
//                 });
//         }

//         IEnumerator ReturnHome(Action onDone)
//         {
//             yield return MoveTo(homeAnchor.position);
//             onDone?.Invoke();
//         }

//         IEnumerator MoveTo(Vector3 target)
//         {
//             animator.SetBool("isWalking", true);
//             while (Vector3.Distance(transform.position, target) > 0.05f)
//             {
//                 transform.position = Vector3.MoveTowards(
//                     transform.position,
//                     target,
//                     moveSpeed * Time.deltaTime
//                 );

//                 // xoay mặt theo hướng đi (cho tự nhiên)
//                 Vector3 dir = (target - transform.position).normalized;
//                 if (dir != Vector3.zero)
//                     transform.rotation = Quaternion.Slerp(
//                         transform.rotation,
//                         Quaternion.LookRotation(dir),
//                         10f * Time.deltaTime
//                     );

//                 yield return null;
//             }
//             animator.SetBool("isWalking", false);
//         }
//     }
// }
