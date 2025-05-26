# practice
獨立開發的一些重要小遊戲功能

這裡是所有小遊戲的重要功能
2048
├──  GameManager.cs             // 管理整個 2048 遊戲流程，包括分數、重新開始、GameOver 顯示等功能
├── // TileBoard.cs             // 管理整個 2048 遊戲的邏輯，包括格子生成、合併邏輯與輸入控制
Times:
├── PlayerController.cs         // 控制玩家移動與記錄位置
├── PathRecorder.cs             // 負責記錄每次通關的完整路徑
├── ShadowGhost.cs              // 影子實際移動與阻擋行為
├── ShadowSpawner.cs            // 控制影子的生成規則與間隔邏輯