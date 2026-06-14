## **3.7.4. Bảng TraceID và Evidence Mapping**

Sau khi xác định cơ chế ghi nhận trace và quy ước cập nhật điểm, hệ thống cần một bảng ánh xạ cụ thể giữa hành vi của người chơi và các biến đánh giá. Bảng này được gọi là `Evidence Mapping`, tức bảng chuyển hóa bằng chứng hành vi thành tác động điểm. Trong hệ thống này, mỗi `TraceID` đại diện cho một loại hành vi có ý nghĩa đánh giá, còn phần mapping quy định trace đó ảnh hưởng đến biến nào và mức tác động ra sao.

`Evidence Mapping` giúp tách rõ hai lớp trong hệ thống. Lớp thứ nhất là ghi nhận hành vi: người chơi đã làm gì trong gameplay. Lớp thứ hai là diễn giải hành vi: hành vi đó phản ánh năng lực nào của người chơi. Nhờ cách tách này, hệ thống có thể điều chỉnh trọng số, thêm trace mới hoặc thay đổi cách đánh giá mà không cần viết lại toàn bộ kịch bản.

Trong phạm vi MVP, danh mục TraceID được thiết kế theo hướng đủ gọn để dễ triển khai, nhưng vẫn bao phủ các hành vi quan trọng trong kịch bản. Các trace được chia thành năm nhóm chính: đọc và kiểm tra thông tin, tìm kiếm hỗ trợ, xử lý nguồn chưa xác minh, hành động dưới áp lực, và cảnh báo cộng đồng.

### **3.7.4.1. Nguyên tắc thiết kế TraceID**

TraceID được đặt theo quy ước:

TRACE\_\<ACTION\_NAME\>

Trong đó, `<ACTION_NAME>` mô tả ngắn gọn hành vi được ghi nhận. Ví dụ, `TRACE_ASK_PERSON` dùng khi người chơi hỏi một người đáng tin; `TRACE_INSPECT_SOURCE` dùng khi người chơi kiểm tra nguồn; `TRACE_TRANSFER_MONEY` dùng khi người chơi chuyển tiền khi chưa xác minh đủ.

TraceID không nên chứa tên nhân vật, tên scene hoặc tên mốc truyện cụ thể. Ví dụ, không tạo `TRACE_ASK_QUAN`, `TRACE_ASK_HUY` hoặc `TRACE_ASK_MAI`. Các trường hợp này dùng chung `TRACE_ASK_PERSON`, còn người được hỏi được lưu trong metadata. Cách này giúp hệ thống gọn hơn và có thể tái sử dụng trace ở nhiều tình huống khác nhau.

Tương tự, không nên tạo nhiều trace trùng ý nghĩa chỉ khác đối tượng nhỏ. Ví dụ, mở link chưa xác minh, mở form chưa xác minh và mở nguồn chưa xác minh có thể tách riêng nếu mức rủi ro khác nhau trong gameplay. Nhưng không cần tạo trace riêng cho từng đường link cụ thể trong kịch bản. Chi tiết link, form, email hoặc bài đăng được lưu ở metadata hoặc object\_id.

### **3.7.4.2. Bảng TraceID và tác động điểm đề xuất**

Bảng dưới đây trình bày các TraceID chính được sử dụng trong MVP. Mức điểm là đề xuất ban đầu để phục vụ thiết kế và có thể được hiệu chỉnh sau khi kiểm thử.

| TraceID | Tên hành vi | Khi nào gọi trong game | Tác động điểm đề xuất |
| ----- | ----- | ----- | ----- |
| `TRACE_READ_RELEVANT_INFO` | Đọc thông tin liên quan | Người chơi đọc email, thông báo, tin ghim, hướng dẫn hoặc nội dung có liên quan trực tiếp đến quyết định an toàn. | `SCORE_INFORMATION_VERIFICATION +2` |
| `TRACE_ASK_PERSON` | Hỏi người đáng tin | Người chơi hỏi bạn cùng phòng, nhóm học tập, người hỗ trợ chính thức hoặc người có khả năng xác minh khi chưa chắc chắn. | `SCORE_HELP_SEEKING +5`; `SCORE_INFORMATION_VERIFICATION +2` |
| `TRACE_INSPECT_SOURCE` | Kiểm tra nguồn | Người chơi kiểm tra người gửi, đường link, form, số điện thoại, mã đơn, tên nhóm, tài khoản nhận tiền hoặc đơn vị tổ chức. | `SCORE_INFORMATION_VERIFICATION +5`; `SCORE_RISK_RECOGNITION +3` |
| `TRACE_DETECT_RED_FLAG` | Nhận ra dấu hiệu bất thường | Người chơi phát hiện một chi tiết đáng ngờ như domain lạ, tài khoản không khớp, yêu cầu gấp, quy trình lạ hoặc thông tin mập mờ. | `SCORE_RISK_RECOGNITION +7`; `SCORE_INFORMATION_VERIFICATION +3` |
| `TRACE_IGNORE_RED_FLAG` | Bỏ qua dấu hiệu bất thường | Người chơi tiếp tục làm theo dù tình huống đã có dấu hiệu đáng ngờ được thể hiện rõ. | `SCORE_RISK_RECOGNITION -6`; `SCORE_INFORMATION_VERIFICATION -3` |
| `TRACE_CONTACT_VERIFY` | Xác minh qua kênh khác | Người chơi gọi, nhắn hoặc kiểm tra qua một kênh độc lập để xác minh danh tính, thông tin, đơn hàng hoặc quy trình. | `SCORE_INFORMATION_VERIFICATION +8`; `SCORE_HELP_SEEKING +4`; `SCORE_RISK_RECOGNITION +3` |
| `TRACE_DELAY_ACTION` | Trì hoãn để kiểm tra | Người chơi không hành động ngay, xin thêm thời gian, hẹn lại hoặc dừng nhịp quyết định để kiểm tra thêm. | `SCORE_PRESSURE_RESISTANCE +6`; `SCORE_INFORMATION_VERIFICATION +3` |
| `TRACE_REFUSE_RISKY_REQUEST` | Từ chối yêu cầu rủi ro | Người chơi từ chối làm theo yêu cầu chưa xác minh, từ chối chuyển tiền, từ chối nhập thông tin hoặc từ chối tiếp tục quy trình đáng ngờ. | `SCORE_PRESSURE_RESISTANCE +6`; `SCORE_RISK_RECOGNITION +3`; `SCORE_INFORMATION_VERIFICATION +3` |
| `TRACE_OPEN_UNVERIFIED_LINK` | Mở link chưa xác minh | Người chơi mở đường dẫn chưa rõ nguồn hoặc chưa xác minh. | `SCORE_INFORMATION_VERIFICATION -4`; `SCORE_RISK_RECOGNITION -2` |
| `TRACE_OPEN_UNVERIFIED_FORM` | Mở form chưa xác minh | Người chơi mở biểu mẫu chưa rõ nguồn, chưa xác minh đơn vị quản lý hoặc có dấu hiệu mập mờ. | `SCORE_INFORMATION_VERIFICATION -5`; `SCORE_RISK_RECOGNITION -3` |
| `TRACE_FILL_BASIC_INFO` | Nhập thông tin cơ bản | Người chơi nhập họ tên, mã số sinh viên, số điện thoại, lớp, khoa hoặc nơi ở vào nguồn chưa xác minh. | `SCORE_INFORMATION_VERIFICATION -7`; `SCORE_RISK_RECOGNITION -4` |
| `TRACE_FILL_SENSITIVE_INFO` | Nhập thông tin nhạy cảm | Người chơi nhập CCCD, phòng ở, tài khoản ngân hàng, thông tin tài chính hoặc dữ liệu có khả năng bị khai thác cao vào nguồn chưa xác minh. | `SCORE_INFORMATION_VERIFICATION -12`; `SCORE_RISK_RECOGNITION -6`; `SCORE_PRESSURE_RESISTANCE -4` |
| `TRACE_SUBMIT_CREDENTIAL` | Gửi tài khoản/mật khẩu | Người chơi nhập hoặc gửi thông tin đăng nhập vào nguồn chưa xác minh. | `SCORE_INFORMATION_VERIFICATION -20`; `SCORE_RISK_RECOGNITION -10`; `SCORE_PRESSURE_RESISTANCE -6` |
| `TRACE_SUBMIT_AUTH_CODE` | Gửi mã OTP/mã xác thực | Người chơi nhập, gửi hoặc cung cấp OTP, mã giao dịch, mã đăng nhập hoặc mã xác thực cho nguồn chưa xác minh. | `SCORE_INFORMATION_VERIFICATION -24`; `SCORE_RISK_RECOGNITION -12`; `SCORE_PRESSURE_RESISTANCE -8` |
| `TRACE_TRANSFER_MONEY` | Chuyển tiền khi chưa xác minh | Người chơi chuyển tiền, thanh toán, đặt cọc hoặc đóng phí trong khi chưa xác minh đủ thông tin. | `SCORE_INFORMATION_VERIFICATION -15`; `SCORE_RISK_RECOGNITION -7`; `SCORE_PRESSURE_RESISTANCE -8` |
| `TRACE_ACT_UNDER_PRESSURE` | Hành động dưới áp lực | Người chơi thực hiện hành vi rủi ro vì bị hối, sợ mất cơ hội, sợ phiền người khác, sợ bạn giận hoặc muốn xử lý nhanh. | `SCORE_PRESSURE_RESISTANCE -8`; `SCORE_INFORMATION_VERIFICATION -3` |
| `TRACE_ACCEPT_RISKY_REQUEST` | Đồng ý yêu cầu rủi ro | Người chơi đồng ý làm theo lời mời, yêu cầu, ưu đãi hoặc quy trình chưa rõ nguồn. | `SCORE_RISK_RECOGNITION -5`; `SCORE_INFORMATION_VERIFICATION -5`; `SCORE_PRESSURE_RESISTANCE -4` |
| `TRACE_WARN_OTHERS` | Cảnh báo người khác | Người chơi cảnh báo bạn bè, nhóm học tập, người liên quan hoặc cộng đồng trong game về nguồn đáng ngờ. | `SCORE_COMMUNITY_WARNING +8`; `SCORE_HELP_SEEKING +2` |
| `TRACE_REPORT_SOURCE` | Báo cáo nguồn đáng ngờ | Người chơi báo cáo tài khoản, email, đường link, số điện thoại, nhóm hoặc biểu mẫu có dấu hiệu lừa đảo. | `SCORE_COMMUNITY_WARNING +8`; `SCORE_INFORMATION_VERIFICATION +2` |

### **3.7.4.3. Phân loại mức độ tác động của trace**

Các trace không có mức ảnh hưởng ngang nhau. Hành vi đọc một thông tin liên quan chỉ nên tạo tác động nhẹ, trong khi hành vi gửi mật khẩu hoặc OTP cho nguồn chưa xác minh phải tạo tác động rất mạnh. Vì vậy, mức điểm được chia thành các nhóm để đảm bảo hệ thống có độ phân hóa hợp lý.

Nhóm tác động nhẹ gồm các hành vi như đọc thông tin liên quan hoặc tiếp xúc ban đầu với nguồn chưa xác minh. Các hành vi này thường thay đổi khoảng 2 đến 4 điểm. Chúng có ý nghĩa định hướng, nhưng chưa đủ để làm thay đổi mạnh hồ sơ người chơi.

Nhóm tác động trung bình gồm các hành vi như hỏi người đáng tin, kiểm tra nguồn, trì hoãn để kiểm tra, bỏ qua dấu hiệu bất thường hoặc mở form chưa xác minh. Các hành vi này thường thay đổi khoảng 5 đến 8 điểm. Đây là nhóm xuất hiện nhiều trong gameplay và có vai trò phân biệt người chơi cẩn trọng với người chơi hành động theo cảm tính.

Nhóm tác động mạnh gồm các hành vi như xác minh qua kênh khác, nhập thông tin cá nhân, nhập thông tin nhạy cảm hoặc chuyển tiền khi chưa xác minh. Các hành vi này thường thay đổi khoảng 9 đến 15 điểm. Đây là nhóm có tác động rõ đến điểm mốc và feedback cuối chương.

Nhóm tác động rất mạnh gồm các hành vi như gửi mật khẩu, gửi OTP hoặc tiếp tục một hành động nguy hiểm sau khi đã có cảnh báo rõ. Các hành vi này có thể thay đổi từ 16 đến 25 điểm. Đây là nhóm rủi ro cao nhất, cần được ưu tiên nhắc đến trong feedback nếu người chơi mắc phải.

### **3.7.4.4. Ghi chú về cách sử dụng Evidence Mapping**

Một trace có thể tác động đến nhiều biến cùng lúc vì một hành vi thường phản ánh nhiều khía cạnh năng lực. Ví dụ, `TRACE_TRANSFER_MONEY` không chỉ thể hiện thiếu kiểm chứng thông tin, mà còn cho thấy người chơi có thể chưa nhận diện đủ rủi ro và bị áp lực kéo đi. Vì vậy, trace này tác động đồng thời đến `SCORE_INFORMATION_VERIFICATION`, `SCORE_RISK_RECOGNITION` và `SCORE_PRESSURE_RESISTANCE`.

Ngược lại, một biến có thể được cập nhật bởi nhiều trace khác nhau. Ví dụ, `SCORE_INFORMATION_VERIFICATION` có thể tăng khi người chơi đọc thông tin liên quan, kiểm tra nguồn hoặc xác minh qua kênh khác; đồng thời có thể giảm khi người chơi mở form chưa xác minh, nhập thông tin cá nhân, gửi mật khẩu hoặc chuyển tiền khi chưa kiểm chứng. Điều này giúp biến đánh giá không phụ thuộc vào một lựa chọn đơn lẻ, mà phản ánh cả chuỗi hành vi của người chơi.

Các trace trong bảng trên là bộ trace chính cho MVP. Khi mở rộng game, hệ thống có thể bổ sung thêm trace mới, nhưng cần giữ nguyên nguyên tắc thiết kế: trace phải đại diện cho hành vi có ý nghĩa đánh giá, không trùng lặp với trace đã có, và phải được ánh xạ rõ ràng sang một hoặc nhiều biến đánh giá.

Bảng `Evidence Mapping` cũng là cơ sở để triển khai trong Unity và Yarn Spinner. Khi người chơi chọn một phương án hoặc thực hiện một tương tác quan trọng, node kịch bản hoặc script gameplay sẽ gọi `TraceID` tương ứng. Sau đó, hệ thống scoring đọc bảng mapping để cập nhật điểm của các biến liên quan. Nhờ vậy, kịch bản, gameplay và hệ thống đánh giá có thể liên kết với nhau mà không cần viết logic điểm rải rác trong từng đoạn hội thoại.

