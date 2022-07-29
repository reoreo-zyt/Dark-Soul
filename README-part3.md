## 1. 目录

![6ad224d3519f69b534be8c93ef0a6563](http://110.40.253.20:5300/api/v1/image/6ad224d3519f69b534be8c93ef0a6563)

* 相机跟随优化
* 相机碰撞处理

## 2. 开发记录

### 2.1 相机跟随弧形衰减

在相机跟随中，弧形衰减要比线性衰减更加自然。

*定义相机跟随速度*

```csharp
private Vector3 cameraFollowVelocity = Vector3.zero;
```

*修改相机跟随逻辑*

```csharp
Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
```

### 2.2 相机碰撞处理

实际上在有障碍物时，由于碰撞的原因，相机没法通过遮挡物看到人物的视角。

*定义变量*

```csharp
        private float targetPosition;  
        private Vector3 cameraFollowVelocity = Vector3.zero;
        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffSet = 0.2f;
        public float minimumCollisionOffset = 0.2f;
```

*控制相机碰撞检测*

```csharp
        private void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if(Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffSet);
            }

            if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 2.0f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
```

*在 fixedUpdate 中执行：*

```csharp
HandleCameraCollisions(delta);