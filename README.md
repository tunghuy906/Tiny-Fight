# Tiny Fight 2D

**Phiên bản Unity:** 2022.3.33f1

## Giới thiệu

Tiny Fight 2D là một game platformer 2D thú vị được phát triển bằng Unity Engine. Người chơi sẽ điều khiển nhân vật chiến đấu qua nhiều màn chơi đầy thử thách, đối mặt với các loại kẻ thù khác nhau và thu thập điểm kỹ năng để nâng cấp nhân vật.

## Tính năng chính

### 🎮 Gameplay
- **Điều khiển mượt mà**: Hệ thống di chuyển, nhảy, dash và tấn công trực quan
- **Hệ thống chiến đấu**: Tấn công cận chiến và tầm xa với projectile
- **Hệ thống leo thang**: Khả năng leo thang và di chuyển dọc
- **Nhiều màn chơi**: 3 màn chơi với độ khó tăng dần (Map 1, Map 2, Map 3)
- **Hệ thống tiến độ**: Phải hoàn thành màn trước để mở khóa màn tiếp theo

### 👾 Kẻ thù đa dạng
- **Bat Enemy**: Kẻ thù bay di chuyển theo waypoints và bắn đạn
- **Boss Bat**: Boss cuối với AI thông minh, có thể:
  - Đuổi theo người chơi
  - Né đạn của người chơi
  - Bắn đạn theo hình tròn và thẳng
  - Di chuyển ngẫu nhiên trong vùng giới hạn
- **Elite Enemy**: Kẻ thù mặt đất với khả năng tấn công cận chiến
- **Slinger Enemy**: Kẻ thù tầm xa có thể phát hiện và bắn người chơi
- **Undead**: Kẻ thù đặc biệt với sức mạnh tăng cường

### 🎯 Hệ thống kỹ năng
- **Skill Points**: Thu thập điểm kỹ năng từ việc tiêu diệt kẻ thù
- **Nâng cấp**: Sử dụng điểm kỹ năng để cải thiện khả năng nhân vật
- **Mana System**: Hệ thống mana để sử dụng tấn công tầm xa

### 🎨 Đồ họa và âm thanh
- **Pixel Art**: Phong cách đồ họa pixel art đẹp mắt
- **Animation**: Hệ thống animation mượt mà cho nhân vật và kẻ thù
- **Sound Effects**: Âm thanh phong phú cho các hành động
- **Background Music**: Nhạc nền tạo không khí cho từng màn chơi

### 🎮 Giao diện người dùng
- **Menu chính**: Giao diện bắt đầu game thân thiện
- **Chọn màn**: Màn hình lựa chọn level với hệ thống khóa
- **Pause Menu**: Có thể tạm dừng game bất cứ lúc nào
- **Settings**: Tùy chỉnh âm lượng và các cài đặt khác
- **Death Panel**: Màn hình khi nhân vật chết với tùy chọn restart

## Cách điều khiển

### 🕹️ Điều khiển cơ bản
- **Di chuyển**: Phím mũi tên hoặc WASD
- **Nhảy**: Spacebar (hỗ trợ double jump)
- **Dash**: Shift để di chuyển nhanh
- **Tấn công cận chiến**: Chuột trái
- **Tấn công tầm xa**: Chuột phải (tiêu tốn mana)
- **Leo thang**: Giữ phím lên khi ở gần thang

### ⚡ Kỹ năng đặc biệt
- **Speed Boost**: Tăng tốc độ di chuyển trong thời gian ngắn
- **Bullet Evade**: Khả năng né đạn thông minh của Boss

## Cài đặt và chạy game

### 📋 Yêu cầu hệ thống
- **Unity Version**: 2022.3.33f1 trở lên
- **Operating System**: Windows, macOS, Linux
- **Input System**: Unity Input System package

### 🔧 Hướng dẫn setup
1. **Clone repository**:
   ```bash
   git clone https://github.com/tunghuy906/Tiny-Fight
   cd Tiny-Fight
   ```

2. **Mở project trong Unity**:
   - Mở Unity Hub
   - Click "Open" và chọn thư mục project
   - Đảm bảo sử dụng Unity 2022.3.33f1

3. **Install packages** (nếu cần):
   - Unity Input System
   - TextMeshPro
   - 2D packages

4. **Build và chạy**:
   - File → Build Settings
   - Chọn platform (Windows, Mac, Linux)
   - Click "Build and Run"

## Cấu trúc Project

```
Assets/
├── Scenes/           # Các scene game
│   ├── Menu.unity    # Menu chính
│   ├── ChoseMap.unity # Chọn màn chơi
│   ├── Map 1.unity   # Màn chơi 1
│   ├── Map 2.unity   # Màn chơi 2
│   └── Map 3.unity   # Màn chơi 3 (Boss fight)
├── Script/           # Code C#
│   ├── Player/       # Script điều khiển người chơi
│   ├── Enemy/        # Script AI kẻ thù
│   ├── UI/          # Script giao diện
│   └── Manager/     # Script quản lý game
├── Prefab/          # Prefab objects
├── Audio/           # File âm thanh
├── Charater/        # Sprite nhân vật
└── Objects/         # Sprite objects và environment
```

## Tính năng kỹ thuật

### 🤖 AI System
- **Pathfinding**: Kẻ thù di chuyển thông minh
- **State Machine**: Hệ thống trạng thái cho AI
- **Detection System**: Phát hiện người chơi trong phạm vi

### 💾 Save System
- **PlayerPrefs**: Lưu tiến độ game và điểm kỹ năng
- **Scene Management**: Quản lý chuyển đổi màn chơi
- **Progress Tracking**: Theo dõi màn đã hoàn thành

### 🎵 Audio System
- **Audio Manager**: Quản lý âm thanh tập trung
- **SFX**: Hiệu ứng âm thanh cho hành động
- **BGM**: Nhạc nền cho từng scene
- **Volume Control**: Điều chỉnh âm lượng

## Đóng góp

Nếu bạn muốn đóng góp vào project:

1. Fork repository
2. Tạo branch mới cho feature (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

## Credits

### 🎨 Assets
- **Pixel Art**: Cainos Pixel Art Platformer - Village Props
- **Characters**: Custom character sprites
- **Audio**: Various sound effects và background music

### 🛠️ Tools
- **Unity Engine**: 2022.3.33f1
- **Input System**: Unity Input System
- **TextMeshPro**: UI text rendering

## License

Project được thực hiện bởi Nhóm 5 - Đồ án chuyên ngành:
- Hoàng Nguyễn Tùng Huy
- Nguyễn Trung Hiếu
- Nguyễn Mạnh Hoàn
- Đinh Duy Thái
- Nguyễn Khải Hưng
## Changelog

### Version 1.0.0
- ✅ Hoàn thành core gameplay
- ✅ 3 màn chơi với Boss fight
- ✅ Hệ thống kỹ năng cơ bản
- ✅ Audio system hoàn chỉnh
- ✅ UI/UX polished

### Planned Updates
- 🔄 Thêm nhiều loại kẻ thù
- 🔄 Hệ thống achievement
- 🔄 More skill upgrades
- 🔄 Additional maps
