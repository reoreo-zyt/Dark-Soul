## 1. 开发目录

![1d33ac525b822f2506c2e3a0999f251d](http://110.40.253.20:5300/api/v1/image/1d33ac525b822f2506c2e3a0999f251d)

* 创建一个可以移动的平台
* 导入玩家角色
* 下载 unity 的 input package
* 配置 unity 的 input system
* 玩家的 Collider 和 Rigidbody
* 编写人物移动、旋转的脚本
* 动画
* 测试移动
* 解决bug

## 2. 开发记录

### 2.1 创建可以移动的平台

*首先创建一个 Plane*

在 Hierarchy 中【右键】-【3D Object】-【Plane】

![](http://110.40.253.20:9002/api/v1/image/6b7ee297043a6f13ee074c25fe55bfba)

![](http://110.40.253.20:9002/api/v1/image/a0f9cc04747b3114fb45340e83c9a697)

*为 Plane 增加材质*

在 Project 中，在 Assets 下新建 Materials 文件夹，在文件夹下【右键】-【Create】-【Material】，命名为 `Floor_Mat`

![](http://110.40.253.20:9002/api/v1/image/6cebd0ffd87aa5a71264eb8961d725b2)

【点击】`Floor_Mat`，选择【Main Maps】中的【Albedo】，选择【Default-Checker-Gray】贴图并将贴图【拖拽】至 Plane 中

设置【Tiling】，将【x】【y】的值修改为5，5

![](http://110.40.253.20:9002/api/v1/image/d85b071324ff3f58569172bbd40fac02)

Plane 效果如下所示：

![](http://110.40.253.20:9002/api/v1/image/ef7f41d841fb08e61aad76bc742bfb4e)

最后，在【Hierarchy】中将 Plane 名称修改为 Floor

### 2.2 导入玩家角色

在该[链接](https://drive.google.com/file/d/1KdBzSiz0rDiZ9X-g2SdfAnj9ebIRVXlV/view)中下载免费的人物模型

在【Project】中的【Assets】新建 Model 目录，用来存放角色模型，将模型命名为 Player

![](http://110.40.253.20:9002/api/v1/image/2ca1c320be61fcb4b54758a4eb37b0b4)

在【Hierarchy】中【右键】-【Create Empty】，并将该空对象命名为 Player，将 Models 中的 Player 模型放置在该父节点上

![](http://110.40.253.20:9002/api/v1/image/c4ffa61ba0df7401c1a037867842c48c)

### 2.3 下载 unity 的 input package

点击【window】-【Package Manager】，找到 Input System 安装并导入

![](http://110.40.253.20:9002/api/v1/image/b0153f4cdbd0229d4db711767eb9426c)

在 Assets 中【右键】-【Create】-【Input Actions】，创建名为 PlayerControls 的 .inputactions 文件

【点击】PlayerControls 文件，在【inspector】中点击【Generate C# Class】，点击【Apply】后将会生成同名的 C# 文件

### 2.4 配置 unity 的 Input System

【双击】PlayerControls 文件或者选择在视图里【Edit asset】，设置其 Maps, Actions, Properties

Maps相当于管理的文件夹，方便管理不同的输入操作

*新建Action Maps，将其命名为Player Movement*

![](http://110.40.253.20:9002/api/v1/image/5334ebc8cd2f79e4b9503715b74fac97)

*新增 Actions，并将其命名为 Movement，将 Action 的Action Type 更改为 Pass Through，将 Control Type 更改为 Vector 2*

![](http://110.40.253.20:9002/api/v1/image/a0f9cc04747b3114fb45340e83c9a697)

*在 Movement 中添加 【Add Up\Down\Left\Right Composition】，命名为 WASD*

![](http://110.40.253.20:9002/api/v1/image/1cc2968720c0af4d1c2f5271841636e3)

*将其绑定的属性选项 Composite 中的 Mode 更改为 Analog，最后将WASD中的值依次绑定键值*

![](http://110.40.253.20:9002/api/v1/image/f4128aadbed133628a8d98da15438ff5)

*新增 Actions，命名为 Camera，设置 Action Type 和 Control type 分别为 Pass Through 和 Vector 2*

![](http://110.40.253.20:9002/api/v1/image/2c9292a5fc369b5c6c985355e23d894f)

*在 Camera 下添加 Binding 的 Path 为 Right Stick [Gamepad]，添加 Processors 的 Stick Deadzone*

![](http://110.40.253.20:9002/api/v1/image/ba63283abcf85c1fe6cf50c153cfb097)

*在 Camera 下添加 Binding 的 Path 为 Delta [Mouse]，添加 Processors 为 Normalize Vector 2*

![](http://110.40.253.20:9002/api/v1/image/355d3d6f0837143ab55bc74f76ae8387)

*点击【edit】-【Project Settings】将 Default Hold Time 设置为 0，减少延时*

![](http://110.40.253.20:9002/api/v1/image/e947289f0e37d97fb9c97039842842f4)

## 2.5 玩家的 Collider 和 Rigidbody

*设置Capsule Collider，使其包裹住玩家的身体*

![](http://110.40.253.20:9002/api/v1/image/f2151e856b23796164173a6fea4aca35)

*设置 Rigidbody，在 Constaints 中勾选上 Freeze Rotation的 X Y Z，这部分我们需要自己处理*

![](http://110.40.253.20:9002/api/v1/image/768ddb5ccd01a4c51e1970a3c53b998f)

## 2.6 实现人物的移动、旋转

*编写 InputHandler 脚本用于检测用户的输入：*

```csharp
using UnityEngine;

namespace ZYT
{
    public class InputHandler : MonoBehaviour
    {
        // 设置为 public 可以在 unity 中进行实时修改
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        // 配置 input systems 时自动生成的类
        PlayerControls inputActions;

        // 用于表示 2D 向量和点。
        Vector2 movementInput;
        Vector2 cameraInput;

        // 关于 unity 事件函数的执行顺序
        // https://docs.unity3d.com/cn/current/Manual/ExecutionOrder.html

        // 加载第一个场景
        // （（仅在对象处于激活状态时调用），在启动对象后立即调用此函数，在创建 MonoBehaviour 实例时会执行此调用）
        public void OnEnable()
        {
            // input system 的文档
            // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/manual/index.html
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                // PlayerMovement 为定义的 Action Maps
                // started performed canceled 是按键输入的三个阶段触发事件

                // !基础：事件与委托 发布者-订阅者模式 匿名方法 Lambda表达式
                // 添加事件处理程序，使用了 Lambda 表达式
                // 每个 Action 都有 started performed canceled 三个回调
                // started 接收到设备的输入
                // performed 交互完成
                // canceled 取消交互
                // ctx.ReadValue 用于读取数据供后续使用
                // !目的为在输入交互完成时实时读取数据
                inputActions.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
            }

            inputActions.Enable();
        }

        // 退出时
        // 行为被禁用或处于非活动状态时，调用此函数。
        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
        }

        private void MoveInput(float delta)
        {
            // ! 注意 unity 中的各种坐标系
            // 关于坐标系的理解：
            // https://blog.csdn.net/ztysmile/article/details/88583067
            // ! 全局坐标系、局部坐标系、屏幕坐标系、视口坐标系
            // 基本数学知识：左手坐标系和右手坐标系
            // https://www.cnblogs.com/aminxu/p/4441016.html
            horizontal = movementInput.x;
            vertical = movementInput.y;
            // https://docs.unity3d.com/cn/2021.3/ScriptReference/Mathf.html
            // Abs 返回绝对值
            // Clamp01 如果值大于 1，返回 1；如果值为负，返回 0。
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            // 屏幕坐标系（通常用于定位鼠标）
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}
```

*编写 AnimatorHandler 实现动画*

```csharp
using UnityEngine;

namespace ZYT
{
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator anim;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            #region Vertical
            float v = 0;

            if(verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if(verticalMovement > 0.55f)
            {
                v = 1;
            }
            else if(verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if(verticalMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }
            #endregion

            #region Horizontal
            float h = 0;

            if(horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if(horizontalMovement > 0.55f)
            {
                h = 1;
            }
            else if(horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if(horizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            #endregion

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }
    }
}
```

*编写 PlayerControls 控制人物的移动、旋转*

```csharp
using UnityEngine;

namespace ZYT
{
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;

        // 隐藏 Inspector 面板中属性的显示
        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        // 额外的，还有锁定相机，魂类游戏视角
        public GameObject normalCamera;

        // 在 Inspector 面板上添加标题
        // 序列化私有字段，在 Inspector 面板上可以实时修改
        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;

        void Start()
        {   
            // GetComponent 获取组件
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            // 获取相机对象
            cameraObject = Camera.main.transform;
            // 获取坐标系信息对象
            myTransform = transform;
            animatorHandler.Initialize();
        }

        public void Update()
        {
            float delta = Time.deltaTime;

            inputHandler.TickInput(delta);

            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            if(animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        // 处理角色的旋转
        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if(targetDir == Vector3.zero)
                targetDir = myTransform.forward;
      
            float rs = rotationSpeed;

            // 四元数
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        #endregion
    }
}
```

*将脚本挂载到角色 Root 中：*

![a0edf4502fd560a4c9f6a35af241fca2](http://110.40.253.20:5300/api/v1/image/a0edf4502fd560a4c9f6a35af241fca2)

![a84395dbcb7102cd0b0e942c671c3f3c](http://110.40.253.20:5300/api/v1/image/a84395dbcb7102cd0b0e942c671c3f3c)

*添加动画组件 Animator ，挂载 AnimatorHandler 脚本在 Player 模型上：*

![519aa9ea96df3424cc7bc30c910bb0ab](http://110.40.253.20:5300/api/v1/image/519aa9ea96df3424cc7bc30c910bb0ab)

*结果：*

![fbe113bfd26cf86d112b7a1f5c80c340](http://110.40.253.20:5300/api/v1/image/fbe113bfd26cf86d112b7a1f5c80c340)

### 2.7 创建动画

动画资源在此处下载：https://drive.google.com/drive/folders/1j2HicZMabg4h2Oe8ocxGNuKBHY5kzFJA

*在 Assets 中添加 Animations 文件夹，导入下载的资源*

![68f4848f413bbe28f64dc102a73ae703](http://110.40.253.20:5300/api/v1/image/68f4848f413bbe28f64dc102a73ae703)

*创建 Animator Controller*

![5b4b5113f196457b3d6de7d1657ab7d3](http://110.40.253.20:5300/api/v1/image/5b4b5113f196457b3d6de7d1657ab7d3)

*将创建的 Animator Controller 命名为 Humanoid 并将其附加到 Player 模型的 Animator 组件中，然后双击该控制器，在 Parameters 面板中定义 vertical 和 Horizontal，两者均为 float 变量。*

![8b50313ae61bc6715b9b575b25e40b99](http://110.40.253.20:5300/api/v1/image/8b50313ae61bc6715b9b575b25e40b99)

*在 Lyaers 面板中新建一个 Blend Tree：*

![0ba0cf44addf733c123b0ae21e038239](http://110.40.253.20:5300/api/v1/image/0ba0cf44addf733c123b0ae21e038239)

![94b9ebf29cc3af78094dead4b0003b92](http://110.40.253.20:5300/api/v1/image/94b9ebf29cc3af78094dead4b0003b92)

*添加动画 Idle、Walk、Run*

![da71760ff923c0b24701ca5c42b07b33](http://110.40.253.20:5300/api/v1/image/da71760ff923c0b24701ca5c42b07b33)

*修改 PlayerLocomotion*

```csharp
public void Update()
        {
            ...
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);
            ...

        }
```

*结果：*

![1d33ac525b822f2506c2e3a0999f251d](http://110.40.253.20:5300/api/v1/image/1d33ac525b822f2506c2e3a0999f251d)

### 2.8 解决 bug

**人物动画没有播放**

将模型改为类人模型：

![04135a9ab9475792e83caa74c881cc11](http://110.40.253.20:5300/api/v1/image/04135a9ab9475792e83caa74c881cc11)
