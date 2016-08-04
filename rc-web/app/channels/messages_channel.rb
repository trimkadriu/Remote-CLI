require "base64"
# Be sure to restart your server when you modify this file. Action Cable runs in a loop that does not support auto reloading.
class MessagesChannel < ApplicationCable::Channel
  def subscribed
    stream_from "messages_#{params[:authid]}"
    # TODO: Implement State
  end

  def unsubscribed
    # TODO: Implement State
  end

  def stream_result(data)
    msg = Base64.decode64(data['message'])
    ActionCable.server.broadcast "messages_#{data["authid"]}", message: msg, user: data['user'], type: data['type']
  end

  def stream_command(data)
    msg = Base64.encode64(data['message'])
    ActionCable.server.broadcast "messages_#{data["authid"]}", message: msg, user: data['user'], type: data['type']
  end

end
