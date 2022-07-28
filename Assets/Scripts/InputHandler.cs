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
        // 控制相机移动旋转
        CameraHandler cameraHandler;

        // 用于表示 2D 向量和点。
        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }

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
