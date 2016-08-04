class ApplicationController < ActionController::Base
  protect_from_forgery with: :exception
  before_filter :authenticate_user!, except: [:index], :unless => [:devise_controller?, :'index']

  def index
    if not user_signed_in?
      redirect_to new_user_session_path and return
    else
      redirect_to dashboard_path
    end
  end
end
