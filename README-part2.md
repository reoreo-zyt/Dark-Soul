## 1. 目录

![15b3c49eaae0607c88559f704a6301b6](http://110.40.253.20:5300/api/v1/image/15b3c49eaae0607c88559f704a6301b6)

* 创建 Camera Handle 对象
* 编写 Camera Handle 对象的脚本

## 2. 开发记录

### 2.1 创建 Camera Handle 对象

*创建两个空节点，命名为 Camera Holder 和 Camera Pivot，将Main Camera放置于其下：*

![5263eb96d1e3de32600dbf5d19e506d7](http://110.40.253.20:5300/api/v1/image/5263eb96d1e3de32600dbf5d19e506d7)

*调整 Main Camera 位置，使其能够以第三人称展示人物：*

![2418a38c1813ee3c3e03d2ed2c902db5](http://110.40.253.20:5300/api/v1/image/2418a38c1813ee3c3e03d2ed2c902db5)

### 2.2 编写 CameraHandler 脚本，控制相机旋转、跟随

*添加 Player 节点的 Controller 层*

![71bd732586507b1b3896040117778234](http://110.40.253.20:5300/api/v1/image/71bd732586507b1b3896040117778234)

*将 layer 层选择为 Controller*

![b0cd6eb6c4edf1d5173f68a898b6f311](http://110.40.253.20:5300/api/v1/image/b0cd6eb6c4edf1d5173f68a898b6f311)

*编写 CameraHandler 脚本，将脚本附加到 Camera Holder 上，并将相关对象的位置信息引入*

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZYT
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        private LayerMask ignoreLayers;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;

        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        public void FollowTarget(float delta)
        {
            Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            myTransform.position = targetPosition;
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta;
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }
}


```

![a07cf99757d3c927aa8c9b533b60b297](http://110.40.253.20:5300/api/v1/image/a07cf99757d3c927aa8c9b533b60b297)