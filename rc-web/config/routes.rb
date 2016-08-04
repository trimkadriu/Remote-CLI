Rails.application.routes.draw do
  resources :messages
  resources :machines
  devise_for :users
  # For details on the DSL available within this file, see http://guides.rubyonrails.org/routing.html
  root to: "application#index"
  get 'dashboard', to: 'dashboard#index'
  get 'machines/:id/remote-control', to: 'machines#remote_control', as: 'machine_remote_control'

  # Resources
  resources :users
  resources :machines
  resources :messages

  # WebSocket
  mount ActionCable.server => '/cable'
end
