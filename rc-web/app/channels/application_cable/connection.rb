module ApplicationCable
  class Connection < ActionCable::Connection::Base
    def receive(websocket_message) #:nodoc:
      puts '=========================='
      puts websocket_message
      send_async :dispatch_websocket_message, websocket_message
    end
  end
end
